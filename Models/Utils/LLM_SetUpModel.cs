using Microsoft.SemanticKernel;

public class LLM_SetupModel
{
    public required string SecretKey { get; set; }
    public required string ModelName { get; set; }
    public required string Endpoint { get; set; }

    public LLM_SetupModel(string secretKey, string modelName, string endpoint)
    {
        SecretKey = secretKey;
        ModelName = modelName;
        Endpoint = endpoint;
    }

    public IKernel Validate()
    {
        //Console.WriteLine("Validating the setup model");
        //Console.WriteLine("Secret Key: " + SecretKey);
        //Console.WriteLine("Model Name: " + ModelName);
        //Console.WriteLine("Endpoint: " + Endpoint);
        if (string.IsNullOrWhiteSpace(SecretKey))
        {
            throw new ArgumentNullException(nameof(SecretKey));
        }
        if (string.IsNullOrWhiteSpace(ModelName))
        {
            throw new ArgumentNullException(nameof(ModelName));
        }
        if (string.IsNullOrWhiteSpace(Endpoint))
        {
            throw new ArgumentNullException(nameof(Endpoint));
        }
        var SKbuilder = new KernelBuilder();
            SKbuilder.WithAzureChatCompletionService(
            ModelName,     // Azure OpenAI Deployment Name
            Endpoint,       // Azure OpenAI Endpoint
            SecretKey);    // Azure OpenAI Key

        IKernel kernel = SKbuilder.Build();
        Console.WriteLine("Kernel built successfully");
        return kernel;
    }
}
