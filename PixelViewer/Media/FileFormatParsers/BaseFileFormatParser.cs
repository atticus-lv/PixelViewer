﻿using Carina.PixelViewer.Media.Profiles;
using CarinaStudio;
using CarinaStudio.IO;
using CarinaStudio.Threading;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Carina.PixelViewer.Media.FileFormatParsers
{
    /// <summary>
    /// Base implementation of <see cref="IFileFormatParser"/>.
    /// </summary>
    abstract class BaseFileFormatParser : IFileFormatParser
    {
        // Fields.
        ILogger? logger;
        
        
        /// <summary>
        /// Initialize new <see cref="BaseFileFormatParser"/> instance.
        /// </summary>
        /// <param name="format">Supported file format.</param>
        protected BaseFileFormatParser(FileFormat format)
        {
            this.Application = format.Application;
            this.FileFormat = format;
        }


        /// <inheritdoc/>
        public IApplication Application { get; }


        /// <inheritdoc/>
        public bool CheckAccess() => this.Application.CheckAccess();


        /// <inheritdoc/>
        public FileFormat FileFormat { get; }


        /// <summary>
        /// Get logger.
        /// </summary>
        protected ILogger Logger
        {
            get
            {
                this.logger ??= this.Application.LoggerFactory.CreateLogger(this.GetType().Name);
                return this.logger;
            }
        }


        /// <inheritdoc/>
        public SynchronizationContext SynchronizationContext => this.Application.SynchronizationContext;


        /// <inheritdoc/>
        public async Task<ImageRenderingProfile?> ParseImageRenderingProfileAsync(IImageDataSource source, CancellationToken cancellationToken)
        {
            // open stream
            this.VerifyAccess();
            Stream? stream;
            try
            {
                stream = await source.OpenStreamAsync(StreamAccess.Read, cancellationToken);
            }
            catch
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();
                return null;
            }

            // parse
            try
            {
                return await ParseImageRenderingProfileAsyncCore(source, stream, cancellationToken);
            }
            catch
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();
                return null;
            }
            finally
            {
                Global.RunWithoutErrorAsync(stream.Close);
            }
        }


        /// <summary>
        /// Called to parse <see cref="ImageRenderingProfile"/> from source.
        /// </summary>
        /// <param name="source">Source of data.</param>
        /// <param name="stream">Stream to read data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task of parsing <see cref="ImageRenderingProfile"/>.</returns>
        protected abstract Task<ImageRenderingProfile?> ParseImageRenderingProfileAsyncCore(IImageDataSource source, Stream stream, CancellationToken cancellationToken);


        /// <summary>
        /// Throw <see cref="ArgumentException"/> for invalid file format.
        /// </summary>
        protected void ThrowInvalidFormatException() => throw new ArgumentException("Invalid format.");
    }
}
