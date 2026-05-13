# PrendeteRock Backend

API REST en C# con .NET 8

## Estructura

- **PrendeteRock.Api** - API REST principal
- **PrendeteRock.Core** - Modelos y lógica de negocio

## Comandos

```bash
# Restaurar dependencias
dotnet restore

# Ejecutar la API (puerto 5000)
dotnet run

# Ejecutar con watch mode (desarrollo)
dotnet watch run
```

## Endpoints

- `GET /api/health` - Verificar estado de la API
