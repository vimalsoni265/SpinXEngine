namespace SpinXEngine.Repository
{
    /// <summary>
    /// Specifies metadata for a class that represents a collection in a data model.
    /// </summary>
    /// <remarks>This attribute is typically used to annotate classes that correspond to collections in a
    /// database. The <see cref="Name"/> property defines the name of the collection.</remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the name associated with the current instance.
        /// </summary>
        public string Name { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        public CollectionAttribute(string name)
        {
            Name = name;
        } 

        #endregion
    }
}
