# Zaglushki

Unity training simulator for industrial valve and flange operations.

## Open the project

1. Install Unity 2022.3.60f1 using Unity Hub.
2. Clone this repository with Git LFS installed, then run `git lfs pull`.
3. Add the repository folder in Unity Hub and open it.
4. Wait for package restoration and asset import.
5. Open `Assets/Scenes/LoginScene.unity`. Scene order is stored in `ProjectSettings/EditorBuildSettings.asset`.

## Backend setup

This project uses Supabase. Configure your own URL and public anon key in Assets/MainScripts/Server/SupabaseConfig.cs. The original server configuration is deliberately replaced with placeholders. Database schema and deployed backend functions are not included; login and server-dependent features require a compatible backend.

## Repository contents

- `Assets/`: scripts, scenes, models and other assets, including Unity metadata.
- `Packages/`: package manifest and embedded dependencies.
- `ProjectSettings/`: Unity project configuration.

Generated caches, IDE files and builds are excluded. Larger assets are stored using Git LFS.

## Third-party content

The project includes third-party assets and packages. Their respective license terms apply. This repository does not grant additional rights to third-party content.

## Validation

Prepared from the local project for portfolio publication. A fresh Unity import and player build have not been verified.

