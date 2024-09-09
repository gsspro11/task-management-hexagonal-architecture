using Poc.Kafka.Common.Constants;

namespace Poc.Kafka.Configs.Validators;

internal static class PocKafkaProducerConfigValidator
{
    internal static void Validate(PocKafkaProducerConfig producerConfig)
    {
        ArgumentNullException.ThrowIfNull(producerConfig);
        ArgumentException.ThrowIfNullOrWhiteSpace(producerConfig.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(producerConfig.Topic);
        ArgumentException.ThrowIfNullOrWhiteSpace(producerConfig.BootstrapServers);

        if (producerConfig.MessageSendMaxRetries < ProducerConstant.MIN_MESSAGE_SEND_RETRIES ||
            producerConfig.MessageSendMaxRetries > ProducerConstant.MAX_MESSAGE_SEND_RETRIES)
            throw new ArgumentException($"The minimum value allowed for the {nameof(producerConfig.MessageSendMaxRetries)} " +
                $"property is {ProducerConstant.MIN_MESSAGE_SEND_RETRIES} and the maximum is {ProducerConstant.MAX_MESSAGE_SEND_RETRIES}. Current value: {producerConfig.MessageSendMaxRetries}.");

        if (producerConfig.EnableIdempotence)
        {
            if (producerConfig.MaxInFlight < ProducerConstant.MIN_IN_FLIGHT_WITH_ENABLE_IDEMPOTENCE ||
                producerConfig.MaxInFlight > ProducerConstant.MAX_IN_FLIGHT_WITH_ENABLE_IDEMPOTENCE)
                throw new ArgumentException($"When {nameof(producerConfig.EnableIdempotence)} is enabled, the minimum value " +
                    $"allowed for the {nameof(producerConfig.MaxInFlight)} property is {ProducerConstant.MIN_IN_FLIGHT_WITH_ENABLE_IDEMPOTENCE} " +
                    $"and the maximum is {ProducerConstant.MAX_IN_FLIGHT_WITH_ENABLE_IDEMPOTENCE}. Current value: {producerConfig.MaxInFlight}.");

            if (producerConfig.MessageSendMaxRetries > ProducerConstant.MAX_MESSAGE_SEND_RETRIES_WITH_ENABLE_IDEMPOTENCE)
                throw new ArgumentException($"When {nameof(producerConfig.EnableIdempotence)} is enabled, the maximum value allowed for the " +
                    $"{nameof(producerConfig.MessageSendMaxRetries)} property is {ProducerConstant.MAX_MESSAGE_SEND_RETRIES_WITH_ENABLE_IDEMPOTENCE}. Current value: {producerConfig.MessageSendMaxRetries}.");

            if (producerConfig.Acks != Confluent.Kafka.Acks.All)
                throw new ArgumentException($"When {nameof(producerConfig.EnableIdempotence)} is enabled, the value for the {nameof(producerConfig.Acks)}" +
                    $" property is required Acks.All. Current value: {producerConfig.Acks}.");
        }

        PocKafkaCredentialsConfigValidator.Validate(producerConfig);
    }
}
