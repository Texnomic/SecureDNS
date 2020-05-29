using System;
using Texnomic.Chaos.NaCl.Base;

namespace Texnomic.Chaos.NaCl
{
    /// <summary>
    /// XChaCha20-Poly1305 AEAD construction, as described in <a href="https://tools.ietf.org/html/draft-arciszewski-xchacha-02">draft</a>.
    /// </summary>
    /// <seealso cref="SnufflePoly1305" />
    public sealed class XChaCha20Poly1305 : SnufflePoly1305
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XChaCha20Poly1305"/> class.
        /// </summary>
        /// <param name="Key">The key.</param>
        public XChaCha20Poly1305(ReadOnlyMemory<byte> Key) : base(Key) { }

        /// <summary>
        /// Creates the snuffle instance.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="InitialCounter">The initial counter.</param>
        /// <returns>Snuffle.</returns>
        protected override Snuffle CreateSnuffleInstance(ReadOnlyMemory<byte> Key, int InitialCounter) => new XChaCha20(Key, InitialCounter);
    }
}
