---
stoplight-id: yhg20jd49tkhw
---

# Quickstart

En esta guía veremos como realizar solicitudes al API CIAM OTP.

**Paso 1: Crear una cuenta en de Spin Plus usando el API CIAM**

Crea una cuenta en el API de CIAM, etc.

**Paso 2: Obten un token mediante el API CIAM**

Usa este token como método método de autenticación de tipo Bearer token.

**Paso 3: Solicita una petición al endpoint Send**

Para hacer una solicitud correcta es necesario el siguiente modelo.

```json json_schema
{
  "type": "object",
  "properties": {
    "email": {
      "type": "string",
    },
    "channel": {
      "type": "string"
    },
    "message": {
      "type": "string"
    },
    "templateId": {
      "type": "string"
    },
    "type": {
      "type": "string"
    },
    "serviceCode": {
      "type": "number"
    }
  }
}
```