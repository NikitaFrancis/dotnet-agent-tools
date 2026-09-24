# 🚀 Autonomous GitHub Triage Agent with .NET 8 & Semantic Kernel

Este proyecto demuestra un flujo de **agente de inteligencia artificial autónomo** desarrollado en **.NET 8** utilizando **Microsoft Semantic Kernel** y **Azure AI Foundry (gpt-4o)**. El agente es capaz de triagelar issues en GitHub, buscar fallas en la base de código, generar parches de corrección automáticamente y abrir Pull Requests con comentarios explicativos.

Presentado en vivo durante el evento **Microsoft NextGen Heroes**.

---

## 🛠️ Tecnologías y Requisitos

- **.NET 8 SDK**
- **Microsoft Semantic Kernel (v1.35.0)**
- **Azure AI Foundry / Azure OpenAI Service** (`gpt-4o` o `gpt-4o-mini`)
- **Octokit** (GitHub API Client para .NET)

---

## ⚙️ Configuración y Ejecución

1. **Clonar el repositorio:**
   ```bash
   git clone [https://github.com/francis-banos/dotnet-agent-tools.git](https://github.com/francis-banos/dotnet-agent-tools.git)
   cd dotnet-agent-tools
Configurar Variables de Entorno o Credenciales:
Abre el archivo Program.cs y reemplaza los marcadores de posición con tus credenciales de Azure OpenAI y tu GitHub Personal Access Token (PAT):

C#
string azureEndpoint = "https://<TU-RECURSO>[.openai.azure.com/](https://.openai.azure.com/)";
string apiKey = "<TU-AZURE-OPENAI-KEY>";
string deploymentName = "gpt-4o";
string githubToken = "ghp_<TU-GITHUB-PAT>";
string owner = "<TU-USUARIO-GITHUB>";
string repo = "<TU-REPOSiTORIO-OBJETIVO>";
Ejecutar la aplicación:

Bash
dotnet run
Probar el Agente:
Ingresa el número de Issue existente en tu repositorio (por ejemplo: 1 o 2) cuando la consola te lo solicite y observa al agente ejecutar el ciclo de razonamiento y Tool Calling autónomo.

🤝 Créditos y Comunidad
Desarrollado como demostración práctica para la comunidad de Microsoft Learn Student Ambassadors y desarrolladores .NET.

Sintámonos libres de hacer Fork, enviar Pull Requests o adaptar este agente a sus propios flujos de integración continua (CI/CD).


---

### Remate para dejar todo preparado esta noche

1. Crea el archivo `README.md` con ese texto en la carpeta `C:\Users\Usuario\NextGenAgentDemo\GitHubTriageAgent`.
2. Sube la carpeta a `[github.com/francis-banos/dotnet-agent-tools](https://github.com/francis-banos/dotnet-agent-tools)` asegurándote de que en `Program.cs` aparezcan los marcadores de posición (por ejemplo `"TU_AZURE_OPENAI_KEY"`) y **no tus claves reales**.
3. Verifica que en tu otro repositorio (`NikitaFrancis/nextgen-agent-demo-repo`) el **Issue #2** esté creado con la falla de multiplicación lista.

¡Con eso tienes la presentación 100% armada y pulida para triunfar en la demo de mañana
