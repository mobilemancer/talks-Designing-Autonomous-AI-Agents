---
theme: quantum
layout: default
transition: fadeIn
---

# Workflow Details

- Executors - The processing units, AI agents or other logic
  - Typed input
  - Produces typed output or events
- Edges - Defines the connections, defines the flow of information
  - Direct
  - Conditional
  - Fan-out, Fan in
- Events - Provides observability
  - Several built in that tracks workflow lifecycle
  - Possible to define own Events
- Workflow - A combination of Executors and Edges
  - Use `WorkFlowBuilder` to wire up Executors and Edges
  