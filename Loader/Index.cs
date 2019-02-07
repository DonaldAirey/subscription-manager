// <copyright file="Index.cs" company="Gamma Four">
//    Copyright © 2018 - Gamma Four.  All rights reserved.
// </copyright>
// <author>Donald Airey</author>
namespace GammaFour.UnderWriter.Loader
{
    /// <summary>
    /// Manages the loading of tables.
    /// </summary>
    public class Index
    {
        /// <summary>
        /// Gets or sets the Web API endpoint.
        /// </summary>
        public string Api { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the table should not be loaded.
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets or sets the field used as a primary key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the message that describes the progress.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the path to the folder.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets verb used in the REST API call.
        /// </summary>
        public Verb Verb { get; set; } = Verb.Put;
    }
}