using TMPro;
using UnityEngine;

public class UserListItemUI : MonoBehaviour
{
    [Header("Texts")]
    public TMP_Text fullNameText;
    public TMP_Text emailText;

    private InstructorUsersUI owner;
    private InstructorUsersUI.UserDto user;

    //выгрузка данных из InstructorUsersUI
    public void Setup(InstructorUsersUI parent, InstructorUsersUI.UserDto dto)
    {
        owner = parent;
        user = dto;

        if (fullNameText != null)
            fullNameText.text = dto.full_name;

        if (emailText != null)
            emailText.text = dto.email;
    }

    public void OnDeleteClicked()
    {

        if (owner == null || user == null)
            return;

        owner.AskDeleteUser(user);
    }

    public void OnResetPasswordClicked()
    {

        if (owner == null || user == null)
            return;

        owner.AskResetPassword(user);
    }
}