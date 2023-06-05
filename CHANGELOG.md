---
stoplight-id: mzgirlt9r1lmv
---

# CHANGELOG CIAM_OTP

En este documento se indican los cambios liberados por versión de la aplicación CIAM_OTP.

Repo: <https://github.com/digitaltitransversal/tr-super-back-ciam-otp-backend-dotnet>

README (Documentación): <https://github.com/digitaltitransversal/tr-super-back-ciam-otp-backend-dotnet#readme>

El proyecto https://github.com/digitaltitransversal/tr-super-back-ciam-otp-backend-dotnet es el encargado de la implementación del servicio One-Time Password (OTP) que integra lo relacionado a los códigos de verificación para la autenticación y autorización de usarios de CIAM, así como la persistencia y validación de la información de los códigos OTP alamacenados en Redis.

## **0.0.9** ~ 2023-05-15
```diff
* Metodos editados    
    + OtpManager.cs
        Se integra el chequeo de whitelist para permitir que números específicos puedan tener OTP con los valores de otp dummy

* Nueva funcionalidad
    ---

* Bugs
    + OtpModel.cs
        Se inicializan los tiempos con un minuto antes del actual; si se validaba y luego pedia un otp, tenia que esperar un minuto


* Documentación Actualizada
```

## **0.0.9** ~ 2023-05-15
```diff
* Metodos editados
    ---

* Nueva funcionalidad
    ---

* Bugs
    + OtpManager.cs
        Se arroja el error obtenido por el ValidateIdentifier, arreglando la inconsistencia

* Documentación Actualizada
```


## **0.0.8** ~ 2023-05-09

```diff
* Metodos editados
    + TypeEmail.cs
        Se hace el cambio del nombre del template a Spin_plus_email_verification
    + appsettings.json
        Se hace el cambio de variables para la configuración del correo
    

* Nueva funcionalidad
    ---

* Bugs
    ---

* Documentación Actualizada
```

## **0.0.7** ~ 2023-05-08

```diff
* Metodos editados
    + HandleGlobalExceptionAsync
        Se hace el cambio de loginformation a logerror para el datadog
    

* Nueva funcionalidad
    ---

* Bugs
    ---

* Documentación Actualizada
```

## **0.0.6** ~ 2023-05-03

```diff
* Metodos editados
    + program.cs
        Se agrega la configuración del serilog y se inicializa
    + appsettings.json
        Se agrega la información necesaria para el funcionamiento del serilog con el datadog
    

* Nueva funcionalidad
    ---

* Bugs
    ---

* Documentación Actualizada
```

## **0.0.5** ~ 2023-05-02

```diff
* Metodos editados
    + .editorconfig
        Se agrega la siguiente propiedad dotnet_diagnostic.SA1204.severity = none
    + HealthController
        Se hace el cambio de la versión a la 0.0.3-alpha.
    + Send
        Se hace el cambio del OtpModel al SendOtpRequest.
    + Managers
        Se hace refactorización del codigo y se quitan las librerias que no se utilizan
    

* Nueva funcionalidad
    + SuperbackCiamOTPTest
        Se aumenta el code coverage al 71.79%, se agregan unit test de conteo de intentos, template id, US whatsapp obligatorio
        + ErrorMiddlewareValidation.cs
        + FakeDatabase.cs
        + HealthValidation.cs

* Bugs
    ---

* Documentación Actualizada
```

## **0.0.4** ~ 2023-04-25

```diff
* Metodos editados
    + Validate
        Se hace la validación del OTP dependiendo si viene el Email o el PhoneNumber.
    + ErrorCodes.cs
        Mapeo de errores correcto en OTP

* Nueva funcionalidad
    + ValidateIdentifier
        Valida el identificador y dependiendo de su estatus regresa true o false.
    + OtpConfiguration
        Se agrega el conteo de intentos de OTP

* Bugs
    ---

* Documentación Actulizada
```

## **0.0.3** ~ 2023-04-19

```diff
* Metodos editados
    ---

* Nueva funcionalidad
    + SuperbackCiamOTP.csproj
        Se agregan las siguientes librerias System.Net.Http versión 4.3.4, System.Text.RegularExpressions versión 4.3.1.

* Bugs
    ---

* Documentación Actualizada
```

## **0.0.2** ~ 2023-04-19

```diff
* Metodos editados
    + ErrorCodes
        Se agrega un nuevo codigo de error para la validación de estos codigos.
    + GenericMessages
        Se agregan generic message para la validación simple de telefono.
        
    

* Nueva funcionalidad
    + CheckCountry
        Se agrega un metodo para revisar la lada del pais y si es permitido.
    + PhoneValidator
        Prueba unitaria para la validación del telefono.
    + UsernameValidator
        Prueba unitaria para la validación del username.

* Bugs
    ---

* Documentación Actualizada
```


## **0.0.1** ~ 2023-04-17

```diff
* Metodos editados
    + PayloadConstants
        Se agregan constantes para el template de otp.
    + SendOtp
        Cambio para el envío de otp con el template.
    

* Nueva funcionalidad
    ---

* Bugs
    ---

* Documentación Actualizada
```