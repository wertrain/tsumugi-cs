﻿using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsumugiRenderer.Engine.Text.ResourceFont
{
    /// <summary>
    /// ResourceFont main loader. This classes implements FontCollectionLoader and FontFileLoader.
    /// It reads all fonts embedded as resource in the current assembly and expose them.
    /// </summary>
    public partial class FileFontLoader : CallbackBase, FontCollectionLoader, FontFileLoader
    {
        private readonly List<ResourceFontFileStream> _fontStreams = new List<ResourceFontFileStream>();
        private readonly List<ResourceFontFileEnumerator> _enumerators = new List<ResourceFontFileEnumerator>();
        private readonly DataStream _keyStream;
        private readonly Factory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceFontLoader"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public FileFontLoader(Factory factory, string fileName)
        {
            _factory = factory;

            if (fileName.EndsWith(".ttf"))
            {
                using (var fileStream = new FileStream(fileName, FileMode.Open))
                {
                    var fontBytes = new byte[fileStream.Length];
                    int numBytesToRead = (int)fileStream.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        int n = fileStream.Read(fontBytes, numBytesRead, numBytesToRead);

                        if (n == 0) break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }

                    var stream = new DataStream(fontBytes.Length, true, true);
                    stream.Write(fontBytes, 0, fontBytes.Length);
                    stream.Position = 0;
                    _fontStreams.Add(new ResourceFontFileStream(stream));
                }
            }

            // Build a Key storage that stores the index of the font
            _keyStream = new DataStream(sizeof(int) * _fontStreams.Count, true, true);
            for (int i = 0; i < _fontStreams.Count; i++)
            {
                _keyStream.Write((int)i);
            }
            _keyStream.Position = 0;

            // Register the 
            _factory.RegisterFontFileLoader(this);
            _factory.RegisterFontCollectionLoader(this);
        }


        /// <summary>
        /// Gets the key used to identify the FontCollection as well as storing index for fonts.
        /// </summary>
        /// <value>The key.</value>
        public DataStream Key
        {
            get
            {
                return _keyStream;
            }
        }

        /// <summary>
        /// Creates a font file enumerator object that encapsulates a collection of font files. The font system calls back to this interface to create a font collection.
        /// </summary>
        /// <param name="factory">Pointer to the <see cref="SharpDX.DirectWrite.Factory"/> object that was used to create the current font collection.</param>
        /// <param name="collectionKey">A font collection key that uniquely identifies the collection of font files within the scope of the font collection loader being used. The buffer allocated for this key must be at least  the size, in bytes, specified by collectionKeySize.</param>
        /// <returns>
        /// a reference to the newly created font file enumerator.
        /// </returns>
        /// <unmanaged>HRESULT IDWriteFontCollectionLoader::CreateEnumeratorFromKey([None] IDWriteFactory* factory,[In, Buffer] const void* collectionKey,[None] int collectionKeySize,[Out] IDWriteFontFileEnumerator** fontFileEnumerator)</unmanaged>
        FontFileEnumerator FontCollectionLoader.CreateEnumeratorFromKey(Factory factory, DataPointer collectionKey)
        {
            var enumerator = new ResourceFontFileEnumerator(factory, this, collectionKey);
            _enumerators.Add(enumerator);

            return enumerator;
        }

        /// <summary>
        /// Creates a font file stream object that encapsulates an open file resource.
        /// </summary>
        /// <param name="fontFileReferenceKey">A reference to a font file reference key that uniquely identifies the font file resource within the scope of the font loader being used. The buffer allocated for this key must at least be the size, in bytes, specified by  fontFileReferenceKeySize.</param>
        /// <returns>
        /// a reference to the newly created <see cref="SharpDX.DirectWrite.FontFileStream"/> object.
        /// </returns>
        /// <remarks>
        /// The resource is closed when the last reference to fontFileStream is released.
        /// </remarks>
        /// <unmanaged>HRESULT IDWriteFontFileLoader::CreateStreamFromKey([In, Buffer] const void* fontFileReferenceKey,[None] int fontFileReferenceKeySize,[Out] IDWriteFontFileStream** fontFileStream)</unmanaged>
        FontFileStream FontFileLoader.CreateStreamFromKey(DataPointer fontFileReferenceKey)
        {
            var index = Utilities.Read<int>(fontFileReferenceKey.Pointer);
            return _fontStreams[index];
        }

        /// <summary>
        /// 
        /// </summary>
        public new void Dispose()
        {
            foreach (var stream in _fontStreams)
            {
                stream.Dispose();
            }

            base.Dispose();
        }
    }
}
