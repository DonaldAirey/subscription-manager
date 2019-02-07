// <copyright file="Verb.cs" company="Gamma Four">
//    Copyright © 2018 - Gamma Four.  All rights reserved.
// </copyright>
// <author>Donald Airey</author>
namespace GammaFour.UnderWriter.Loader
{
    /// <summary>
    /// The verbs used to all the REST API.
    /// </summary>
    public enum Verb
    {
        /// <summary>
        /// Delete the record in the URL.
        /// </summary>
        Delete,

        /// <summary>
        /// Get one or all records.
        /// </summary>
        Get,

        /// <summary>
        /// Add the record in the body of the message.
        /// </summary>
        Post,

        /// <summary>
        /// Add or Update the record in the body of the message to the URL.
        /// </summary>
        Put
    }
}