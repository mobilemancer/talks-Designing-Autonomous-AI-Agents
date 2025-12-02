---
theme: quantum
layout: default
transition: fadeIn
---

# Microsoft Agent Framework - More Agent Types

| Underlying Inference Service | Description                                                                                       | Service Chat History storage supported | Custom Chat History storage supported |
| ---------------------------- | ------------------------------------------------------------------------------------------------- | -------------------------------------- | ------------------------------------- |
| OpenAI ChatCompletion        | An agent that uses the OpenAI ChatCompletion service.                                             | No                                     | Yes                                   |
| OpenAI Responses             | An agent that uses the OpenAI Responses service.                                                  | Yes                                    | Yes                                   |
| OpenAI Assistants            | An agent that uses the OpenAI Assistants service.                                                 | Yes                                    | No                                    |
| Any other ChatClient         | Any other Microsoft.Extensions.AI.IChatClient implementation. | Varies                                 | Varies                                |
