# PedGPT 🤖

---

🛠️ **Very WIP**

The project has only just begun, so it's not in a real usable state. Drastic changes are expected.

---

PedGPT is an ambitious open-source project that aims to create intelligent autonomous agents for peds (pedestrians) in Grand Theft Auto V (GTA V) using advanced language models like GPT-4, built on .NET. This project takes inspiration from [langchain](https://github.com/hwchase17/langchain), [AutoGPT](https://github.com/Significant-Gravitas/Auto-GPT), and the [ReAct framework](https://arxiv.org/abs/2210.03629) to create peds that can reason, act, and interact in realistic and engaging ways.

The primary goal of this project is to enhance the gaming experience in GTA V by introducing AI-driven peds that possess more complex decision-making and behavior patterns compared to the original game NPCs. The integration with GTA V will be done through a FiveM server via gRPC. 

Work has begun to move on over to using [Semantic Kernel](https://learn.microsoft.com/en-us/semantic-kernel/overview).

**Overall process:**

<img src="https://github.com/MickMelon/PedGPT/assets/21023513/d4c2b963-7e40-42ad-a06b-681136aa212c" width="300" alt="Overall process">

**Signal processing:**

<img src="https://github.com/MickMelon/PedGPT/assets/21023513/392e6fd8-c5dd-4fb2-ad36-12ba3ab3f90a" width="300" alt="Signal processing">

**Deep reasoning:**

<img src="https://github.com/MickMelon/PedGPT/assets/21023513/72842d1b-495a-4d2f-84fb-eb41fc29c773" width="300" alt="Deep reasoning">

# Getting started

⚠️**There is no current limit or real measure to token usage**, so be careful!

1. Clone this repository to your local machine:

```bash
git clone https://github.com/MickMelon/PedGPT.git
```

2. Navigate to the `IntelliPed.ConsoleApp` project folder:

```bash
cd src/IntelliPed.ConsoleApp
```

3. Set your OpenAI API key to your dotnet user secrets by running the following command:

```bash
dotnet user-secrets set "OpenAi:ApiKey" "{your api key}"
dotnet user-secrets set "OpenAi:OrgId" "{your org id}"
```

4. Build the solution using the following command:

```bash
dotnet build
```

5. Start PedGPT by running the following command:

```bash
dotnet run
```

# Contribute

Contributions are welcomed to help improve PedGPT. To contribute:

1. Fork the repository.
2. Create a new branch for your feature or bugfix.
3. Make changes and commit them to your branch.
4. Submit a pull request, detailing the changes you made and why.

# Licence

PedGPT is released under the [GNU GPLv3 Licence](LICENCE.md).
