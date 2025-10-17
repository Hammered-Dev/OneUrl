# OneUrl: Your Single Hub 🔗
OneUrl is a platform to store and share all your beloved links, offerring single and clean link for every access. 

![.NetBadge](https://img.shields.io/badge/.Net-512BD4?logo=dotnet&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?logo=redis&logoColor=white)
![Status](https://img.shields.io/badge/Status-Under%20Development-red)
![License](https://img.shields.io/badge/License-GPLv3-blue)
![Contribution Welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen)

The project is developed with .NET.

**Status**: Under development🚧. We wellcome everyone to join and share your ideas with us.

## Getting started
### Prerequisites
You will need following services to use OneUrl.
#### Runing manually
- Redis
- An OAuth provider (See [Authencation](#authentication))
#### Docker (or other container platform)
- Redis
- An OAuth Provider (See [Authencation](#authentication))

> ⚠️ **Note:** The project's container setup is primarily tested on **Docker**, if you are using other platform, you may need to troubleshoot platform-specific issues yourself.

### Installation
#### Manually
> ⚠️ **Caution:** The manaul setup is experimental. Incorrect settings may affect your system environment.
1. Go to [release page](link.to.release) and download the latest realease executable.
2. Set the environment variables, the following variables is required:
    - AUTH_ISSUER
    - AUTH_AUDIENCE
    - REDIS_HOST
    - REDIS_PORT
    - API_URL
    - AUTH_DOMAIN
    - AUTH_CLIENTID
    - AUTH_CLIENT_SECRECT
    - AUTH_REDIRECT_URI
3. Run the excutable, and the dashboard is avaliable on `localhost:5000`.
#### Docker Compose (Highly recommended)
```yaml
version: "3.8"
service:
    api:
        image: <api-image>
        environment:
            AUTH_ISSUER=<OAuth domain>
            AUTH_AUDIENCE=<OAuth ClientID>
            REDIS_HOST=redis
            REDIS_PORT=6379
        depends_on:
            - redis
        restart: always
    web:
        image: <web-image>
        environment:
            API_URL=http://api:8080
            AUTH_DOMAIN=<OAuth domain>
            AUTH_CLIENTID=<OAuth ClientID>
            AUTH_CLIENT_SECRECT=<OAuth Client Secrect>
            AUTH_REDIRECT_URI=<OAuth Redirect Uri>
        ports:
            - 5000:5000
        depends_on:
            - api
        restart: always
    redis:
        image: redis:latest
        restart: always
```
### Authentication
OneUrl requires an OAuth 2.0 (OpenID Connect) compatible provider for user loggin and authentication.
- Recommendations
    - **Managed Service:** **Auth0** is a good choise if you're just starting.
    - **Self-Hosted:** You can self-host one with projects like [Identity Server from Duende](https://duendesoftware.com/products/identityserver).

## Contributing
Your contribution is highly valued and important for the project's improvement!
### Reporting Issues
If you find an issue, please head to the issue page and open a new issue. Here is what we want you to specify:

1. **Version:** What version you're using?
2. **Platform/OS:** What platform or operation system your using? (e.g. Windows, Mac, Ubuntu)
3. **Container Context:** If you are using Docker or other container platform, please specify the environment. (e.g., Docker on WSL, Docker on Linux).
4. **The Problem:** Describe what happened clearly and concisely.
5. How do you discover it? Trigger some errors while using, reading the source code, etc.
6. **Oter informations:** Any screnshots, logs, or stack traces that can help clarify the problem.

### Submitting a Pull Request
We welcome all features ideas and bug fixes! Please make sure your PR includes the following:
1. **Purpose:** The title should clearly state the feature or problem you're working on. If it's related to an open issue, please **hashtag the issue number**.
2. **Key changes?** Describe the main changes you made, highlighting the affected file names or the most important code modification.
3. **Demonstration** Provide screenshots, GIFs, or logs that demonstrate how your code works and the fix is successful.

### Development Prerequisites
Please read our detailed development guide for setting up the environment: [Development.md](./Development.md).

### Code of Conduct
To ensure a welcoming community, please adhere to our [CODE_OF_CONDUCT.md](link.to.codeofconduct).

## License
The project is open-sourced under [GLP v3 license](linktolincense).