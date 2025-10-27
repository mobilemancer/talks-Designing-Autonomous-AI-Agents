---
theme: frost
layout: default
---

# Tools

- Any function is a Tool
- Use `AIFunctionFactory.Create()`
- Use `DescriptionAttribute` to clarify properties etc for the LLM

## Using Agents as Tools

- Use any defined agent as a tool
- Call `.AsAIFunction()` on an `AIAgent` and provide it as a tool to an agent

## Built in Tools

- `CodeInterpreterToolDefinition` built into Foundry
- _more_
