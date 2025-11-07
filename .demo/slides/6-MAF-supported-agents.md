---
theme: quantum
layout: default
transition: fadeIn
---

# Microsoft Agent Framework - Agent Types

| Underlying Inference Service | Description                                                                                       | Service Chat History storage supported | Custom Chat History storage supported |
| ---------------------------- | ------------------------------------------------------------------------------------------------- | -------------------------------------- | ------------------------------------- |
| Azure AI Foundry Agent       | An agent that uses the Azure AI Foundry Agents Service as its backend.                            | Yes                                    | No                                    |
| **Azure AI Foundry Models**      | An agent that uses any of the models deployed in the Azure AI Foundry Service as its backend.     | No                                     | Yes                                   |
| Azure OpenAI ChatCompletion  | An agent that uses the Azure OpenAI ChatCompletion service.                                       | No                                     | Yes                                   |
| Azure OpenAI Responses       | An agent that uses the Azure OpenAI Responses service.                                            | Yes                                    | Yes                                   |
| OpenAI ChatCompletion        | An agent that uses the OpenAI ChatCompletion service.                                             | No                                     | Yes                                   |
| OpenAI Responses             | An agent that uses the OpenAI Responses service.                                                  | Yes                                    | Yes                                   |
| OpenAI Assistants            | An agent that uses the OpenAI Assistants service.                                                 | Yes                                    | No                                    |
| Any other ChatClient         | You can also use any other Microsoft.Extensions.AI.IChatClient implementation to create an agent. | Varies                                 | Varies                                |
| ---------------------------- | ------------------------------------------------------------------------------------------------- | -------------------------------------- | ------------------------------------- |
