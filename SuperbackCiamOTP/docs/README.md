---
stoplight-id: 1ojgyt42uqv69
---

# Superback CIAM OTP 
[![Run tests and checks](https://github.com/digitaltitransversal/tr_superback_ciam_otp/actions/workflows/ci.yml/badge.svg)](https://github.com/digitaltitransversal/tr_superback_ciam_otp/actions/workflows/ci.yml)

Implementación del servicio One-Time Password (OTP) que integra lo relacionado a los códigos de verificación para la autenticación y autorización de usarios de CIAM, así como la persistencia y validación de la información de los códigos OTP alamacenados en Redis..


## Ejecución local del repositorio
Para ejecutar de forma local, se pueden seguir los siguientes pasos:

#### Preparando el entorno en Redis Docker

1. Instalar docker versión 3.6 o superior (actualmente solo se puede ejecutar Redis en la versión 3.6 en local con Mac), haz click <a href="https://docs.docker.com/desktop/previous-versions/3.x-mac/">aquí</a>.
2. Instalar Homebrew(opcional):
</br><code>/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"</code>
3. Instalar Redis Stack:
</br><code>brew tap redis-stack/redis-stack</code></br><code>brew install --cask redis-stack</code>
4. Para validar que Redis Stack funcione puede ejecutar el siguiente comando, incia el servicio de Redis Stack:
</br><code>redis-stack-server</code>
5. La integración de Redis Stack con Docker requiere ejecutar el siguiente comando:
</br><code> docker run -d --name redis-stack -p 6379:6379 -p 8001:8001 redis/redis-stack:latest</code>

Nota: Para mayor información puede consultar las siguientes urls:<a href="https://developer.redis.com/create/docker/redis-on-docker">Redis on Docker</a> - <a href="https://developer.redis.com/create/redis-stack/">Redis Stack</a>

#### Preparando el entorno en Visual Studio

1. Instalar la versión más reciente de Visual Studio.
2. Descargar el .zip de la rama main del repositorio.
3. Abrir el proyecto haciendo click en el .sln o desde Visual Studio dar click en la opción de abrir.
4. Agregar en el appsettings.Development.json los valores de Redis (este archivo debe cubrir las mismas propiedades que appsettings.json). Si deseas probar con la versión del servidor de Redis, pregunta al equipo de Superback para que te proveen de valores específicos.
6. Iniciar Docker, debera estar ejecutandose el contenedor de Redis Stack.
7. En el dashboard de Docker debe dar click en Containers/Apps, debe validar que el contenedor de Redis Stack debe estar ejecutandose, si no es así deberá iniciar el contenedor.
<img width="1270" alt="Captura de pantalla 2023-02-15 a la(s) 11 26 21" src="https://user-images.githubusercontent.com/123478957/219106276-26d44b82-846b-453b-a534-f2459b950b3f.png">

#### Ejecución del proyecto SuperbackCiamOTP
1. Una vez preparado los entornos de Docker y Visual Studio. Ahora, puedes hacer click en la ejecutar el proyecto, o bien, en "Depurar > Iniciar depuración".

Idealmente se te debe de abrir una ventana en tu navegador como la siguiente:
![Captura de pantalla 2023-02-15 a la(s) 15 40 59](https://user-images.githubusercontent.com/123478957/219174653-ac13b1b9-6e8f-42e1-8496-e3a5cb30a413.png)

Donde puedes interactuar con los endpoints del OTP. Si no se abre la ventana, puedes ingresar a esta por medio del siguiente enlace: https://localhost:7124/swagger/index.html.


#### Ejecutar la CLI de Redis

1. Si requiere ejecutar la CLI de Redis, puede ejecutar el siguiente comando:
</br><code> docker exec -it redis-stack redis-cli</code>
2. Para salir de la CLI de redis, ejecute el siguiente comando:
</br><code>control + c</code>

#### Instalar REDISINSIGHT

1. Puede instalar la GUI Redis en la siguiente url, <a href="https://redis.com/redis-enterprise/redis-insight/?_ga=2.43576166.254717836.1676417031-696077901.1675021118&_gl=1*iek4f5*_ga*Njk2MDc3OTAxLjE2NzUwMjExMTg.*_ga_8BKGRQKRPV*MTY3NjQ4NDIxMi4zNy4wLjE2NzY0ODQyMTIuNjAuMC4w">aquí</a> 


## Como contribuir al repositorio

Para contribuir al repositorio se te deben otorgar permisos de escritura y seguir los siguientes pasos. 

1. Asociar un nuevo SSH con tu cuenta de GitHub institucional.

    En esta ocasión, no basta con descargar el `.zip` del repositorio. Debes configurar tu dispositivo para conectar tu cuenta de GitHub mediante SSH, puedes seguir el siguiente [tutorial de GitHub](https://docs.github.com/en/authentication/connecting-to-github-with-ssh/adding-a-new-ssh-key-to-your-github-account) para añadir una SSH a tu cuenta. Además, debes darle permisos para acceder a `digitaltitransversal`, sigue el siguiente [tutorial](https://docs.github.com/en/enterprise-cloud@latest/authentication/authenticating-with-saml-single-sign-on/authorizing-an-ssh-key-for-use-with-saml-single-sign-on) para dar dichos permisos.

2. Teniendo ya el SSH configurado, debes clonar el repositorio en la carpeta de tu preferencia. Como sugerencia, puedes crear una carpeta `~/Projects` para mantener todos los proyectos en un sólo lugar.

    En la terminal, corre el siguiente comando:
    ```
    git@github.com:digitaltitransversal/tr_superback_ciam_otp.git
    ```

    Debería aparecer una salida parecida a:
    ```
    Cloning into 'tr_superback_ciam_otp'...
    remote: Enumerating objects: 542, done.
    remote: Counting objects: 100% (314/314), done.
    remote: Compressing objects: 100% (177/177), done.
    remote: Total 542 (delta 196), reused 178 (delta 122), pack-reused 228
    Receiving objects: 100% (542/542), 110.60 KiB | 1.44 MiB/s, done.
    Resolving deltas: 100% (271/271), done.
    ```

3. En este punto, se pueden seguir los pasos que se describieron para la ejecución local.

4. Si se quiere agregar nuevos cambios, debes crear tu propia rama. La recomendación para nombrar ramas es agregar el identificador de la tarea y un comentario breve que la describe, por ejemplo, para la tarea [SPPP-254](https://digitalfemsa.atlassian.net/browse/SPPP-254) la rama podría llamarse `SPPP-254-Readme-OTP`.

    ```
    git branch rama-de-ejemplo
    ```

5. Para empezar a trabajar sobre la rama creada, puedes ejecutar el siguiente comando:

    ```
    git checkout rama-de-ejemplo
    ```

    En este punto ya puedes comenzar a hacer tus cambios. Si no haces el `checkout` a tu rama, puede que afectes a otras ramas e interrumpir el trabajo de los demás. Recuerda que no te dejará hacer `push` a `main`. 

6. Después de hacer tus cambios, haz pruebas unitarias en <a href="https://github.com/digitaltitransversal/tr_superback_ciam_otp/tree/main/SuperbackCiamOTPTest">SuperbackCiamOTPTest</a> en la medida de lo posible.

7. Se checa de manera automática que se cumpla ciertas reglas de sintaxis. Algunas de estas reglas son necesarias para tener una sintaxis consistente en todo el repositorio. La mayoría de las reglas se pueden aplicar de manera automática corriendo el siguiente comando:
    ```
    dotnet format
    ```
    En caso de enviar tus cambios y todavía tener pendientes de sintaxis, tendrás que checar de forma manual cómo arreglarlo. Te puedes guiar de la [documentación de StyleCop](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/tree/master/documentation) y pedir ayuda al equipo. Para checar de forma local la sintaxis, puedes correr el siguiente comando:

    ```
    dotnet format --verify-no-changes --verbosity m --severity warn
    ```
## Endpoints

Hasta el momento, se cuentan con los siguientes endpoints:
- `Send`: Genera un código OTP y lo envía al canal designado por el request.
  - Requiere seis `strings`: `PhoneNumber`, `Email`, `Channel`, `Message`, `Type` y `ServiceCode`.
  - Regresa un objeto con los siguientes propiedades: `PhoneNumber`, `Email`, `Channel`, `Message`, `Type` y `ServiceCode`.
  
- `Validate`: Valida el código OTP generado.
  - Requiere tres `strings`: `Otp`, `PhoneNumber` y `Email`.
  - Regresa un objeto con los siguientes propiedades: `Otp`, `PhoneNumber` y `Email`.
