namespace Poc.Kafka.Configurators;

/// <summary>
/// Facilitates the fluent configuration of Poc.Kafka settings, allowing for modular and clear setup of Kafka cluster configurations. 
/// This interface supports chaining configurations to streamline the setup process of multiple Kafka clusters or settings within a single configuration session.
/// </summary>
public interface IPocKafkaConfigurator
{
    /// <summary>
    /// Adds a new Kafka cluster configuration using a specified cluster name and a configuration action. 
    /// The cluster name serves as a unique identifier within the setup, and the configuration action provides a detailed configuration process using an IClusterConfigurator.
    /// </summary>
    /// <param name="clusterName">The unique name assigned to the Kafka cluster. This name is utilized throughout the configuration to reference the specific cluster setup.</param>
    /// <param name="configureAction">A delegate that configures the Kafka cluster. This action provides access to an IClusterConfigurator instance, enabling detailed and specific cluster configuration.</param>
    /// <returns>The IPocKafkaConfigurator instance, supporting fluent chaining of configuration methods.</returns>
    /// <remarks>
    /// This method is central to setting up Kafka clusters, allowing for detailed customization and setup of individual cluster configurations.
    /// The fluent interface design promotes a clean and intuitive approach to configuring multiple aspects or clusters in a coherent manner.
    /// </remarks>
    IPocKafkaConfigurator AddCluster(string clusterName, Action<IClusterConfigurator> configureAction);
}
