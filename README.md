# OneUrl
OneUrl is a platform to store and share all your beloved links. It store the links and let users access them with a single url. The project is developed with .NET. The project is still under development🚧, we wellcome everyone to join and share your ideas with us.

## Getting started
### Prerequisites
If your running the project manually your have to install and run the excutable. However, if you preferred running OneUrl on Docker or other container platforms, just make sure you have them installed.
#### Runing manually
- Redis
- An OAuth provider (See [Authencation](#authentication))
#### Docker (or other container engine)
- Redis
- An OAuth Provider (See [Authencation](#authentication))

> **⚠️ The project's container will only be tested on Docker, if you are using other platform, you may need to deal with the platform specific problems yourself.**

### Installation
#### Manually
> ⚠️ The startup script is under process, you might end up messing up your system environment variables.
1. Go to release page and download the latest realease installer.
2. Set the envirement varibles.
3. Run, and the dashboard is avaliable on `localhost:5000`
#### Docker Compose (Highly recommended)
```yaml
service:
    api:
        image: img1
        environment:
            AUTH_ISSUER=<OAuth domain>
            AUTH_AUDIENCE=<OAuth ClientID>
            REDIS_HOST=
            REDIS_PORT=
    web:
        image: img2
        environment:
            API_URL=api
            AUTH_DOMAIN=<OAuth domain>
            AUTH_CLIENTID=<OAuth ClientID>
            AUTH_CLIENT_SECRECT=<OAuth Client Secrect>
            AUTH_REDIRECT_URI=<OAuth Redirect Uri>
```
### Authentication
The project right now required an OAuth provider, you will need to configure an OAuth provider for logging in. You can use any authentication services that are capatible with OAuth 2.0 (Open id connect). If you don't know where to start, Auth0 is a good choise, or you can self host one with [Identity Server from Duende](https://duendesoftware.com/products/identityserver).

## Contributing
### Issues
If you find an issue, please head to the issue page and open a new issue. Here is what we want you to specify:

1. What version you're using?
2. What platform/OS your using? Windows, Mac, Ubuntu, Docker, etc.
3. If your using Docker or other container platform, please specify the platform your using (Docker on WSL, Docker on Linux, etc).
4. What happen? Describe what happen with short but detailed words.
5. How do you discover it? Trigger some errors while using, reading the source code, etc.
6. Oter informations. Any other informations that would help to clarify the problem. Screnshots, logs, etc.

### Pull Request
You're contribution is very important for the project to improve. If you have some great idea or you want to fix some issues, you're welcome to open a pull request. Please provide the informations below.
1. What is the main purpose? Please tell us what feature you're working on or what problems you solved in short on the PR title. If it's about an opened issue, please also hashtag the issue number.
2. What are the key changes? Describe the changes you made, you may also mark the file name or provide the most important code.
3. Oter informations. Any other informations that show how your code work. Screnshots, logs, etc.

### Dev Prerequisites
Please read [develop.md](link.to.develop.md).

### Code of Conduct
Please read the [CODE_OF_CONDUCT.md](link.to.codeofconduct).

## License
The project is open sourced under [GLP v3 license](linktolincense).