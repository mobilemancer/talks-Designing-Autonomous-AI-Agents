using System.Collections.Generic;

namespace AgentsDemo.Models;

public enum Model
{
    GPT4o,
    GPT41,
    GPT41mini,
    GPT5mini,
}

public static class ModelCatalog
{
    private static readonly IReadOnlyDictionary<Model, string> DeploymentNames = new Dictionary<
        Model,
        string
    >
    {
        { Model.GPT4o, "gpt-4o" },
        { Model.GPT41, "gpt-4.1" },
        { Model.GPT41mini, "gpt-4.1-mini" },
        { Model.GPT5mini, "gpt-5-mini" },
    };

    public static string GetDeploymentName(Model model) => DeploymentNames[model];
}
