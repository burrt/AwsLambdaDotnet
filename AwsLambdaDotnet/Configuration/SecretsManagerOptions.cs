﻿namespace AwsLambdaDotnet.Configuration
{
    public record SecretsManagerOptions
    {
        public required string SecretName { get; init; }
    }
}
