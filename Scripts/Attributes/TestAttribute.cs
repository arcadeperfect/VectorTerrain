using System;
using UnityEngine;

namespace VectorTerrain.Scripts.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class TestAttribute : PropertyAttribute
    {
        /// <summary>
        ///   <para>The minimum allowed value.</para>
        /// </summary>
        public readonly float min;
        
        /// <summary>
        ///   <para>Attribute used to make a float or int variable in a script be restricted to a specific minimum value.</para>
        /// </summary>
        /// <param name="min">The minimum allowed value.</param>
        public TestAttribute(float min) => this.min = min;
    }
}