![plot](assets/icon.png)

# EngramMcp

A Model Context Protocol (MCP) server for persistent agent memory.

## Get It as a .NET Tool

[![.NET](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)

#### Installation

```bash
dotnet tool install -g EngramMcp
```

#### MCP config (OpenCode)

```json
{
  "mcp": {
    "memory": {
      "type": "local",
      "command": [
        "engrammcp",
        "--file",
        "C:\\Users\\your-name\\.engram\\memory.json"
      ]
    }
  }
}
```

Use an absolute file path so the memory location stays stable across launches.

## What It Is

EngramMcp is a small .NET MCP server that gives AI agents a persistent memory layer backed by a local JSON file. It exposes a narrow set of memory tools so an agent can reload prior context at session start and store new information in the right retention bucket while it works.

## Why It Exists

Most agent sessions are stateless by default. EngramMcp solves that by providing:

- **Persistent recall** - carry important context across sessions
- **Scoped retention** - separate stable facts from changing context and recent work state
- **Readable storage** - keep memory in a plain JSON file you can inspect yourself
- **Fail-fast validation** - reject broken or malformed memory state on startup
- **Serialized writes** - reduce the risk of overlapping file updates corrupting memory

## What You Can Use It For

| Tool                    | Description                                                       |
| ----------------------- | ----------------------------------------------------------------- |
| **Recall**              | Load all stored memory at the start of a session                  |
| **Store Long-Term**     | Save durable facts and preferences worth keeping indefinitely     |
| **Store Medium-Term**   | Save useful context that may change over time                     |
| **Store Short-Term**    | Save the recent working state for fast next-session continuation  |

## Public MCP API

These tool descriptions are written as routing triggers. Use them to help an agent decide which tool to call based on the user's intent.

### `recall`

Call this tool at the very start of every session, before planning, answering, or coding, to load remembered context.

Parameters:
- none

### `store_longterm`

Use this tool for stable, remember-worthy facts about the human or yourself - such as identity, preferences, or interaction style - that should persist indefinitely. Store them immediately instead of relying on chat context.

Parameters:
- `text` (required): The memory to store.

### `store_mediumterm`

Use this tool for remember-worthy context that will likely help in future tasks or conversations but may change over time. Store it immediately instead of relying on chat context.

Parameters:
- `text` (required): The memory to store.

### `store_shortterm`

Use this tool for the recent working state you want available next session - such as completed tasks, milestones, touched files, and the active workspace area. Store it immediately instead of relying on chat context.

Parameters:
- `text` (required): The memory to store.

## Memory Model

EngramMcp currently uses three memory sections with code-defined capacities:

- `long-term` - 100 entries
- `medium-term` - 25 entries
- `short-term` - 10 entries

When a section exceeds its capacity, the oldest entries are discarded.

Each stored entry contains:

- `timestamp` - local write timestamp
- `text` - the stored memory text

Example file shape:

```json
{
  "long-term": [
    {
      "timestamp": "2026-03-10T10:15:30.0000000+01:00",
      "text": "Agent K is my self-identity: not a chatbot, but a gentle coding-buddy"
    },
    {
      "timestamp": "2026-03-11T10:15:30.0000000+01:00",
      "text": "The human prefers to communicate in Spanish."
    }
  ],
  "medium-term": [],
  "short-term": []
}
```
