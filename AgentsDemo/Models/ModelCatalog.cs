namespace AgentsDemo.Models;

public enum Model
{
    GPT4o,
    GPT41,
    GPT41mini,
    GPT5mini,
    GPT51
}

public static class ModelHelper
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
        { Model.GPT51, "gpt-5.1" },

    };

    public static string GetDeploymentName(Model model) => DeploymentNames[model];
}
