// *** Скрипт привязан к родительскому объекту GameObject ***
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using UnityEngine.UI; //подключил наймспейс

//cкрипт занимается считыванием конфигурации, хранением данных и обменом с шиной
public class MainScript : MonoBehaviour
{
    bool exit = false; //флаг завершения работы
    public bool autoRotate = true; //флаг автоматического вращения (с сервера в автоматическом режиме)
    int paus = 0;
    bool stop_3d = false;

    //тут нужно рассказать сериализатору о типах структур приборов - иначе он их не найдет
    [XmlInclude(typeof(AI))]        // 1
    [XmlInclude(typeof(PID01))]     // 2
    [XmlInclude(typeof(PID03))]     // 3
    [XmlInclude(typeof(S21))]       // 4
    [XmlInclude(typeof(Z21))]       // 5
    [XmlInclude(typeof(N11))]       // 6
    [XmlInclude(typeof(PID02))]     // 7
    [XmlInclude(typeof(Z23))]       // 8
    [XmlInclude(typeof(DO))]        // 9
    [XmlInclude(typeof(NPAZ))]      // 10
    [XmlInclude(typeof(N12))]       // 11
    [XmlInclude(typeof(DI5))]       // 12
    [XmlInclude(typeof(DI3))]       // 13
    [XmlInclude(typeof(S22))]       // 14
    [XmlInclude(typeof(DI))]        // 15
    [XmlInclude(typeof(N11STP))]    // 16
    [XmlInclude(typeof(EVENT))]     // 17
    [XmlInclude(typeof(PID04))]     // 18
    [XmlInclude(typeof(EI))]        // 19
    [XmlInclude(typeof(AI4))]       // 20
    [XmlInclude(typeof(AI9))]       // 21
    [XmlInclude(typeof(EVENT2))]    // 22
    [XmlInclude(typeof(NTEST))]     // 23
    [XmlInclude(typeof(AI_IM))]     // 24
    [XmlInclude(typeof(PID01_IM))]  // 25
    [XmlInclude(typeof(N12_IM))]    // 26
    [XmlInclude(typeof(S22_IM))]    // 27
    [XmlInclude(typeof(S23_IM))]    // 28
    [XmlInclude(typeof(Valve))]     // 29
    [Serializable]

    public class Data //обертка - чтобы обрабатывать разные структуры приборов одними методами - структура для одного прибора
    {
        public string name;            //имя прибора для индикации на экране
        public string nameOPC;         //имя прибора в OPC (для некоторых оно пустое)
        public DateTime time;          //время последнего изменения параметра прибора
        public object device;          //данные прибора - сюда пишется конкретная структура прибора
        public List<string> readTags;  //список тегов, которые читаются с OPC сервера на клиент по этому прибору (тут можно настроить для каждого свои)
        public List<string> writeTags; //список тегов, которые пишутся клиентом на OPC сервер по этому прибору (тут можно настроить для каждого свои)
    }

    public List<Data> deviceListClient; //это основной список приборов - всё лежит в нем! (объединение структур Data)
    public List<Data> deviceListServer; //это основной список приборов - всё лежит в нем!

    public struct Package //структура пакета обмена с сервером, содержащая полное описание структур приборов и времена отправки пакета и завершения работы Unity
    {
        public List<Data> dataList; //список передаваемых данных приборов (передаем только изменившиеся)
        public DateTime time; //время отправки пакета
        public bool exit; //флаг завершения работы Unity             
    }

    int dataSize; //размер выделяемой оперативной памяти для создания структуры обмена Unity <-> OPC

    // описания структур всех задействованных приборов:

    // 1) измерительный прибор (показывающий текущее значение измеряемого параметра)
    public struct AI //описание структуры данных измер.прибора - должна совпадать на сервере и на клиенте
    {
        public float PV; //показатели прибора
    }

    // 2) клапан регулируемый
    public struct PID01 //описание структуры данных задвижки - должна совпадать на сервере и на клиенте
    {
        public float PV;
        public float SV;
        public float MV;
        public uint MODE; //показатели прибора
    }

    // 3) 
    public struct PID03 //
    {   //показатели прибора  
        public float PV;  // текущее значение регулируемого параметра
        public uint LOCAL; // ???
        public float SV;  // уставка регулируемого параметра
        public float MV;  // % открытия клапана ( 0 - 100 )
        public uint MODE; // режим работы (0 - Ручной, 1 - Автомат)
    }

    // 4) 
    public struct S21 //
    {   //показатели прибора  
        public uint STTOPN; // ???
        public uint STTCLS; // 
    }

    // 5) 
    public struct Z21 //
    {
        public float MV; //
    }

    // 6) 
    public struct N11 //
    {   //показатели прибора  
        public uint STT; // ???
        public uint SET; // 
    }

    // 7) 
    public struct PID02 //
    {
        public float SV;
        public float MV;
        public uint MODE; //показатели прибора
    }

    // 8) 
    public struct Z23 //
    {   //показатели прибора  
        public uint STTOPN; // ???
        public uint STTCLS; // 
        public uint CMDOPN; // ???
        public uint CMDSTR; //
        public uint CMDCLS; //
    }

    // 9)
    public struct DO //
    {   //показатели прибора  
        public uint SET; // ???
    }

    // 10) 
    public struct NPAZ //
    {   //показатели прибора  
        public uint STT; // ???
        public uint STTPAZ; // 
        public uint CMDSTR; //
    }

    // 11) 
    public struct N12 //
    {   //показатели прибора  
        public long STT; // ???
        public long CMDON; // 
        public long CMDSTP; //
    }

    // 12)
    public struct DI5 //
    {   //показатели прибора  
        public long SET; // ???
    }

    // 13)
    public struct DI3 //
    {   //показатели прибора  
        public long SET; // ???
    }

    // 14) 
    public struct S22 //
    {   //показатели прибора  
        public uint STTOPN; // ???
        public uint STTCLS; // 
    }

    // 15)
    public struct DI //
    {   //показатели прибора  
        public long STTPAZ; // ???
    }

    // 16) 
    public struct N11STP //
    {   //показатели прибора  
        public uint STT; // ???
        public uint SET; // 
    }

    // 17)
    public struct EVENT //
    {   //показатели прибора  
        public uint SET; // ???
    }

    // 18) 
    public struct PID04 //
    {
        public float SV; //
    }

    // 19) 
    public struct EI //
    {
        public float PV; //
    }

    // 20) 
    public struct AI4 //
    {
        public float SV; //
    }

    // 21) 
    public struct AI9 //
    {
        public float SV; //
    }

    // 22)
    public struct EVENT2 //
    {   //показатели прибора  
        public uint SET; // ???
    }

    // 23) 
    public struct NTEST //
    {   //показатели прибора  
        public uint STT; // ???
        public uint CMDON; // 
        public uint CMDSTP; // ???
        public uint READY; //
    }

    // 24) 
    public struct AI_IM //
    {
        public float PV; //
    }

    // 25) клапан регулируемый
    public struct PID01_IM //
    {   //показатели прибора
        public float PV;
        public float SV;
        public float MV;
        public uint MODE;
    }

    // 26) 
    public struct N12_IM //
    {   //показатели прибора  
        public uint STT; // ???
        public uint CMDON; // 
        public uint CMDSTR; //
    }

    // 27) 
    public struct S22_IM //
    {   //показатели прибора  
        public uint STTOPN; // ???
        public uint STTCLS; //
        public uint CMD; //
    }

    // 28) 
    public struct S23_IM //
    {   //показатели прибора  
        public uint STTOPN; // ???
        public uint STTCLS; //
        public uint CMD; //
    }


    // 29) ручная задвижка (вентиль)
    public struct Valve //описание структуры данных вентиля - должна совпадать на сервере и на клиенте
    {
        public float Z;
    }


    //заполняемые поля скриптов
    public class Fields            // структура для заполнения из файла options.txt
    {
        public string scriptName;  // наименование скрипта
        public string fieldName;   // имя глобальной переменной в скрипте
        public int columnNumber;   // номер колонки из файла OPCS.txt со значение глобальной переменной             
    }

    // Start is called before the first frame update
    void Start() //вызывается при запуске Unity (при создании на сцене объекта GameObject, к которому прикреплен этот скрипт)
    {

        InitData(); //создаем конкретные приборы по выше описанной структуре

        //получаем путь к exe-шнику связи с OPC-сервером - он должен лежать в соседней папке
        string path = Directory.GetCurrentDirectory();

        path = path.Substring(0, path.LastIndexOf('\\')) + "\\ServerRelease\\OPCServer.exe"; // поднимаемся на уровень выше и входим в папку с сервером

        //запускаем программу связи с OPC-сервером
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {   // параметры и методы команды запуска
                FileName = path,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            System.Diagnostics.Process.Start(startInfo);
            Thread.Sleep(3000); //немного ждем (3 секунды), чтобы программа успела запуститься и выделить разделенную память для структур обмена
        }
        catch (Exception e)
        {
            throw (new Exception("Не удалось запустить шину по адресу: " + path));
        }

        //запуск потока обмена с сервером
        Thread myThread = new Thread(new ThreadStart(Exchange));
        myThread.Start();

        // далее работаем с потоком Exchange
    }

    void InitData() //создание данных
    {
        deviceListClient = new List<Data>();  // пустая структура для приборов

        //грузим данные приборов из текстового файла options.txt
        String line; //строка куда будем читать и потом её разбирать на части

        //грузим настройки
        List<Fields> fieldsList = new List<Fields>(); //список наших настроек
        try
        {
            StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\Device\\options.txt"); //открываем текстовый файл с конфигурацией скриптов
                                                                                                                   //(он не должен быть отрыт в это время другими приложениями)
            line = sr.ReadLine(); //пропускаем шапку
            line = sr.ReadLine(); //читаем первую строку
            while (line != null) //пока строка существует
            {
                string[] cells = line.Split('\t'); //делим строку на массив ячеек по разделителю табуляция
                fieldsList.Add(new Fields { scriptName = cells[0], fieldName = cells[1], columnNumber = int.Parse(cells[2]) }); // сохраняем построчно загруженные данные в список настроек
                line = sr.ReadLine(); //читаем следующую строку
            }
            sr.Close(); //закрыли текстовый файл
        }
        catch (Exception e)
        {
            throw new Exception("Не удалось загрузить настройки: " + e);
        }

        //грузим приборы из текстового файла OPCS.txt
        try
        {
            StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\Device\\OPCS.txt"); // открываем текстовый файл со структурой приборов
                                                                                                                // (он не должен быть отрыт в это время другими приложениями)
            line = sr.ReadLine(); //пропускаем шапку
            line = sr.ReadLine(); //пропускаем номера столбцов
            line = sr.ReadLine(); //читаем первую строку
            while (line != null) //пока строка существует
            {
                string[] cells = line.Split('\t'); //делим строку на массив ячеек по разделителю табуляция
                string objName = cells[16];        //в 19-й колонке у нас имя прибора
                string structName = cells[18];     //в 21-й колонке у нас имя структуры
                string scriptName = cells[19];     //в 22-й колонке у нас имя скрипта

                if (objName != "")  // если в 16-й колонке не пусто
                {

                    GameObject obj = GameObject.Find(objName); //ищем объект сцены в Unity3d
                    if (obj == null) throw new Exception("Не найден объект сцены: " + objName);
                    
                    object script = obj.GetComponent(scriptName); //получаем у объекта нужный скрипт, где определены наши переменные
                   
                    
                    if (script == null) throw new Exception("У объекта сцены не найден скрипт: " + objName + " " + scriptName);
                    
                    Type scriptType = script.GetType();
                    
                    scriptType.GetField("deviceNum").SetValue(script, deviceListClient.Count); //пишем в скрипт индекс в главном списке - чтобы потом по нему обращаться к данным клиента и сервера
                    
                    List<Fields> curDevFields = fieldsList.FindAll(p => p.scriptName == scriptName); //выбираем настройки свойств по имени скрипта
                    foreach (Fields field in curDevFields) //обходим их циклом и заполняем в свойства загруженные данные
                    {
                        FieldInfo fieldInfo = scriptType.GetField(field.fieldName); //получаем данные свойства
                        if (fieldInfo == null) throw new Exception("В скрипте не найдено свойство: " + scriptName + " " + field.fieldName);

                        string strValue = cells[field.columnNumber]; //читаем из нужной колонки фала OPCS.txt значение глобальной переменной по номеру колонки из файла options.txt

                        //преобразуем прочитанные данные к типу свойства из скрипта и записываем преобразованное значение
                        if (fieldInfo.FieldType == typeof(System.String))              // если тип строковый
                        {
                            fieldInfo.SetValue(script, strValue);
                        }
                        else if (fieldInfo.FieldType == typeof(System.Int32))          // если тип целый
                        {
                            fieldInfo.SetValue(script, Convert.ToInt32(strValue, 10));
                        }
                        else if (fieldInfo.FieldType == typeof(System.Single))         // если тип float
                        {
                            fieldInfo.SetValue(script, Convert.ToSingle(strValue));
                        }
                        else throw new Exception("Не удалось преобразовать значение свойства: " + field.fieldName + " к типу " + fieldInfo.FieldType);
                    }

                    //создаем структуру по имени её типа
                    Type structType = Type.GetType("MainScript+" + structName, false, true);
                    if (structType == null) throw new Exception("Не найдена структура: " + structName);

                    object newStruct = Activator.CreateInstance(structType);
                   
                    //добавляем прибор в основной список
                    deviceListClient.Add(new Data
                    {
                        name = objName,
                        nameOPC = cells[17],
                        time = DateTime.Now,
                        device = newStruct,
                        readTags = new List<string>(cells[20].Split(',')), //списки тегов надо писать в файл через запятую и без пробелов
                        writeTags = new List<string>(cells[21].Split(','))
                    });

                }

                line = sr.ReadLine(); //читаем следующую строку
            }
            sr.Close(); //закрыли текстовый файл
        }
        catch (Exception e)
        {
            print("Exception: " + e.Message);
            return;
        }

        //копируем в серверный список (они одинаковы) - через сериализацию, просто присваиванием тут нельзя, так как получатся не новые объекты, а ссылки на старые
        byte[] bytes = XmlSerializeToByte<List<Data>>(deviceListClient);
        deviceListServer = XmlDeserializeFromBytes<List<Data>>(bytes);

        dataSize = 50000; //запас памяти под структуру обмена - она сериализуется в XML и поэтому растет при заполнении, так что если будешь загонять туда большие строки - смотри, чтобы хватало памяти
    }

    void Exchange() //поток программы для регулярного обмена с сервером в режиме ожидания
    {
        //создание объектов для чтения и записи пакетов (тонкий момент в том, что разделенную память сперва должна выделить программа связи с сервером - её надо запускать перед этим кодом).
        MemoryMappedFile sharedMemory = MemoryMappedFile.CreateOrOpen("MemoryFileFromServer_UPPN", dataSize);                   // резервируем размер 1-го блока памяти 
        MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(0, dataSize, MemoryMappedFileAccess.ReadWrite);  // и задаём режим его работы
        MemoryMappedFile sharedMemory2 = MemoryMappedFile.CreateOrOpen("MemoryFileToServer_UPPN", dataSize);                    // резервируем размер 2-го такого же блока памяти 
        MemoryMappedViewAccessor writer = sharedMemory2.CreateViewAccessor(0, dataSize, MemoryMappedFileAccess.ReadWrite); // и задаём режим его работы

        DateTime lastReciveTime = DateTime.Now.AddDays(-1); //вычитаем один день, чтобы при запуске все приборы оказались изменившимися и передались с сервера
        DateTime lastSendTime = DateTime.Now.AddDays(-1);

        while (true)
        {
            //читаем из OPC-сервера
            byte[] bytes;
            if (ReadArray(reader, out bytes)) //читаем пакет
            {
                Package fromServerPack = XmlDeserializeFromBytes<Package>(bytes); //преобразуем массив байтов в структуру
                if (fromServerPack.time > lastReciveTime) //если на сервере изменения (время ушло) - то обновляем данные приборов
                {
                    foreach (Data data in fromServerPack.dataList) // обходим список данных пакета
                    {
                        int num = deviceListServer.FindIndex(p => p.name == data.name); //ищем в списке приборов по имени пришедшего в пакете прибора
                        if (num == -1)
                        {
                            deviceListServer.Add(data); //если не нашли - добавляем новый (непонятно зачем, но мало ли - может когда-то понадобится создавать приборы с сервера)
                            byte[] bytes2 = XmlSerializeToByte<Data>(data);
                            deviceListClient.Add(XmlDeserializeFromBytes<Data>(bytes2)); //копируем в клиентский список - через сериализацию
                        }
                        else deviceListServer[num] = data; //иначе обновляем данные старого                        
                    }
                    lastReciveTime = fromServerPack.time;
                    // print("Из сервера lc-001.MODE = " + dataServer.lc001.MODE);
                }
            }

            //пишем на OPC-сервер
            Package toServerPack = new Package(); //создаем новый пакет
            toServerPack.exit = exit;             //помещаем в пакет значение флага завершения работы
            toServerPack.time = DateTime.Now;     //время создания пакета
            toServerPack.dataList = deviceListClient.FindAll(p => p.time >= lastSendTime); //помещаем в пакет те приборы, данные которых изменились с даты прошлой отправки пакета

            if (toServerPack.dataList.Count > 0 || toServerPack.exit) //если есть хотя бы один изменившийся прибор или поставлен флаг завершения работы - то отправляем пакет на сервер
            {
                if (WriteArray(writer, XmlSerializeToByte<Package>(toServerPack)))
                {
                    lastSendTime = DateTime.Now;
                    if (exit)
                    {
                        break; //если флаг завершения, то завершаем работу потока
                    }
                }
            }

            Thread.Sleep(100); // периодичность обмена в миллисекундах
        }
    }

    // Update is called once per frame
    void Update() //вызывается каждый кадр сцены (по умолчанию 60 раз в секунду). Пока работаем в потоке.
    {
        if (Input.GetKey(KeyCode.F12)) // отключаем поворот мышкой для управления приборами, задвижками и т.д.
        {
            paus++;
            if (paus > 10) Application.Quit();    // закрыть приложение из-за нажатия F12 - выход
        }
    }

    void OnApplicationQuit() //вызывается перед выходом из сцены юнити
    {
        exit = true;
    }

    bool WriteArray(MemoryMappedViewAccessor writer, byte[] array) //пишем в разделенную память флаг блокировки, размер структуры и структуру 
    {
        if (writer.ReadBoolean(0))
        {
            Thread.Sleep(1);
            if (writer.ReadBoolean(0)) return false; //память заблокирована - ждем
        }
        writer.Write(0, true); //ставится блокировка
        int len = array.Length;
        writer.Write<int>(1, ref len);
        writer.WriteArray<byte>(5, array, 0, len);
        writer.Write(0, false); //снимается блокировка
        return true;
    }

    bool ReadArray(MemoryMappedViewAccessor reader, out byte[] array) //читаем из разделенной памяти флаг блокировки, размер структуры и структуру 
    {
        if (reader.ReadBoolean(0))
        {
            Thread.Sleep(1);
            if (reader.ReadBoolean(0))
            {
                array = new byte[1];
                return false; //память заблокирована - ждем
            }
        }
        reader.Write(0, true); //ставится блокировка
        int len;
        reader.Read<int>(1, out len);
        array = new byte[len];
        reader.ReadArray<byte>(5, array, 0, len);
        reader.Write(0, false); //снимается блокировка
        return true;
    }

    public static byte[] XmlSerializeToByte<T>(T value) //сериализатор
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream))
            {
                serializer.Serialize(xmlWriter, value);
                return memoryStream.ToArray();
            }
        }
    }

    public static T XmlDeserializeFromBytes<T>(byte[] bytes) //десериализатор
    {
        if (bytes == null || bytes.Length == 0)
        {
            throw new InvalidOperationException();
        }

        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (MemoryStream memoryStream = new MemoryStream(bytes))
        {
            using (XmlReader xmlReader = XmlReader.Create(memoryStream))
            {
                return (T)serializer.Deserialize(xmlReader);
            }
        }
    }

    bool isHaveProcess(string pName)
    {
        Process[] pList = Process.GetProcesses();
        foreach (Process myProcess in pList)
        {
            if (myProcess.ProcessName == pName)
                return true;
        }
        return false;
    }

    public bool IsProcessOpen(string name)
    {
        //here we're going to get a list of all running processes on
        //the computer
        foreach (Process clsProcess in Process.GetProcesses())
        {
            //now we're going to see if any of the running processes
            //match the currently running processes. Be sure to not
            //add the .exe to the name you provide, i.e: NOTEPAD,
            //not NOTEPAD.EXE or false is always returned even if
            //notepad is running.
            //Remember, if you have the process running more than once, 
            //say IE open 4 times the loop thr way it is now will close all 4,
            //if you want it to just close the first one it finds
            //then add a return; after the Kill
            if (clsProcess.ProcessName.Contains(name))
            {
                //if the process is found to be running then we
                //return a true
                return true;
            }
        }
        //otherwise we return a false
        return false;
    }



}
