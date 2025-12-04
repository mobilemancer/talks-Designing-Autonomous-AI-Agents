---
theme: quantum
layout: default
transition: fadeIn
---

# Microsoft Agent Framework - Agent Types

| Underlying Inference Service | Description                                                                                       | Service Chat History storage supported | Custom Chat History storage supported |
| ---------------------------- | ------------------------------------------------------------------------------------------------- | -------------------------------------- | ------------------------------------- |
| Microsoft Foundry Agent       | An agent that uses the Microsoft Foundry Agents Service as its backend.                            | Yes                                    | No                                    |
| **Microsoft Foundry Models**      | An agent that uses any of the models deployed in the Microsoft Foundry Service as its backend.     | No                                     | Yes                                   |
| Azure OpenAI ChatCompletion  | An agent that uses the Azure OpenAI ChatCompletion service.                                       | No                                     | Yes                                   |
| Azure OpenAI Responses       | An agent that uses the Azure OpenAI Responses service.                                            | Yes                                    | Yes                                   |
