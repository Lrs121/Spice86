namespace Spice86.Aeon.Emulator.Video
{
    using Spice86.Shared;

    /// <summary>
    /// Emulates the VGA DAC which provides access to the palette.
    /// </summary>
    public class Dac
    {
        private readonly Rgb[] _palette = new Rgb[256];
        private int _readChannel;
        private int _writeChannel;
        private byte _readIndex;
        private byte _writeIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dac"/> class.
        /// </summary>
        public Dac()
        {
            Reset();
        }

        /// <summary>
        /// Gets the full 256-color palette.
        /// </summary>
        public ReadOnlySpan<Rgb> Palette => _palette.AsSpan();

        /// <summary>
        /// Gets or sets the current palette read index.
        /// </summary>
        public byte ReadIndex
        {
            get => _readIndex;
            set
            {
                _readIndex = value;
                _readChannel = 0;
            }
        }
        
        /// <summary>
        /// Gets or sets the current palette write index.
        /// </summary>
        public byte WriteIndex
        {
            get => _writeIndex;
            set
            {
                _writeIndex = value;
                _writeChannel = 0;
            }
        }

        /// <summary>
        /// Reads the next channel in the current color.
        /// </summary>
        /// <returns>Red, green, or blue channel value.</returns>
        public byte Read()
        {
            Rgb color = _palette[_readIndex];
            _readChannel++;
            switch (_readChannel)
            {
                case 1:
                    return color.G;
                case 2:
                    return color.B;
            }

            _readChannel = 0;
            _readIndex++;
            return color.R;
        }
        
        /// <summary>
        /// Writes the next channel in the current color.
        /// </summary>
        /// <param name="value">Red, green, or blue channel value.</param>
        public void Write(byte value) {
            _writeChannel++;
            Rgb color = _palette[_writeIndex];
            switch (_writeChannel)
            {
                // value * 255 / 63, or else colors are way too dark on screen
                // We could shift by 2 instead, but while it's faster,
                // it may not be as accurate.
                case 1:
                    color.G = (byte)(value * 255 / 63);
                    break;
                case 2:
                    color.B = (byte)(value * 255 / 63);
                    break;
                default:
                    color.R = (byte)(value * 255 / 63);
                    _writeChannel = 0;
                    _writeIndex++;
                    break;
            }
        }
        
        /// <summary>
        /// Resets the colors to the default 256-color VGA palette.
        /// </summary>
        public void Reset()
        {
            ReadOnlySpan<byte> source = DefaultPalette;
            for (int i = 0; i < 256; i++)
            {
                byte r = source[i * 3];
                byte g = source[i * 3 + 1];
                byte b = source[i * 3 + 2];
                _palette[i] = new Rgb() {
                    R = r,
                    G = g,
                    B = b
                };
            }
        }
        
        /// <summary>
        /// Sets a color to the specified RGB values.
        /// </summary>
        /// <param name="index">Index of color to set.</param>
        /// <param name="r">Red component.</param>
        /// <param name="g">Green component.</param>
        /// <param name="b">Blue component.</param>
        public void SetColor(byte index, byte r, byte g, byte b)
        {
            Rgb item = _palette[index];
            item.R = (byte) (r & 0x3F);
            item.G = (byte) (g & 0x3F);
            item.B = (byte) (b & 0x3F);
        }

        #region DefaultPalette
        private static ReadOnlySpan<byte> DefaultPalette => new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0xA8, 0x00, 0xA8, 0x00, 0x00, 0xA8, 0xA8, 0xA8, 0x00, 0x00, 0xA8,
                0x00, 0xA8, 0xA8, 0x54, 0x00, 0xA8, 0xA8, 0xA8, 0x54, 0x54, 0x54, 0x54, 0x54, 0xFC, 0x54, 0xFC,
                0x54, 0x54, 0xFC, 0xFC, 0xFC, 0x54, 0x54, 0xFC, 0x54, 0xFC, 0xFC, 0xFC, 0x54, 0xFC, 0xFC, 0xFC,
                0x00, 0x00, 0x00, 0x14, 0x14, 0x14, 0x20, 0x20, 0x20, 0x2C, 0x2C, 0x2C, 0x38, 0x38, 0x38, 0x44,
                0x44, 0x44, 0x50, 0x50, 0x50, 0x60, 0x60, 0x60, 0x70, 0x70, 0x70, 0x80, 0x80, 0x80, 0x90, 0x90,
                0x90, 0xA0, 0xA0, 0xA0, 0xB4, 0xB4, 0xB4, 0xC8, 0xC8, 0xC8, 0xE0, 0xE0, 0xE0, 0xFC, 0xFC, 0xFC,
                0x00, 0x00, 0xFC, 0x40, 0x00, 0xFC, 0x7C, 0x00, 0xFC, 0xBC, 0x00, 0xFC, 0xFC, 0x00, 0xFC, 0xFC,
                0x00, 0xBC, 0xFC, 0x00, 0x7C, 0xFC, 0x00, 0x40, 0xFC, 0x00, 0x00, 0xFC, 0x40, 0x00, 0xFC, 0x7C,
                0x00, 0xFC, 0xBC, 0x00, 0xFC, 0xFC, 0x00, 0xBC, 0xFC, 0x00, 0x7C, 0xFC, 0x00, 0x40, 0xFC, 0x00,
                0x00, 0xFC, 0x00, 0x00, 0xFC, 0x40, 0x00, 0xFC, 0x7C, 0x00, 0xFC, 0xBC, 0x00, 0xFC, 0xFC, 0x00,
                0xBC, 0xFC, 0x00, 0x7C, 0xFC, 0x00, 0x40, 0xFC, 0x7C, 0x7C, 0xFC, 0x9C, 0x7C, 0xFC, 0xBC, 0x7C,
                0xFC, 0xDC, 0x7C, 0xFC, 0xFC, 0x7C, 0xFC, 0xFC, 0x7C, 0xDC, 0xFC, 0x7C, 0xBC, 0xFC, 0x7C, 0x9C,
                0xFC, 0x7C, 0x7C, 0xFC, 0x9C, 0x7C, 0xFC, 0xBC, 0x7C, 0xFC, 0xDC, 0x7C, 0xFC, 0xFC, 0x7C, 0xDC,
                0xFC, 0x7C, 0xBC, 0xFC, 0x7C, 0x9C, 0xFC, 0x7C, 0x7C, 0xFC, 0x7C, 0x7C, 0xFC, 0x9C, 0x7C, 0xFC,
                0xBC, 0x7C, 0xFC, 0xDC, 0x7C, 0xFC, 0xFC, 0x7C, 0xDC, 0xFC, 0x7C, 0xBC, 0xFC, 0x7C, 0x9C, 0xFC,
                0xB4, 0xB4, 0xFC, 0xC4, 0xB4, 0xFC, 0xD8, 0xB4, 0xFC, 0xE8, 0xB4, 0xFC, 0xFC, 0xB4, 0xFC, 0xFC,
                0xB4, 0xE8, 0xFC, 0xB4, 0xD8, 0xFC, 0xB4, 0xC4, 0xFC, 0xB4, 0xB4, 0xFC, 0xC4, 0xB4, 0xFC, 0xD8,
                0xB4, 0xFC, 0xE8, 0xB4, 0xFC, 0xFC, 0xB4, 0xE8, 0xFC, 0xB4, 0xD8, 0xFC, 0xB4, 0xC4, 0xFC, 0xB4,
                0xB4, 0xFC, 0xB4, 0xB4, 0xFC, 0xC4, 0xB4, 0xFC, 0xD8, 0xB4, 0xFC, 0xE8, 0xB4, 0xFC, 0xFC, 0xB4,
                0xE8, 0xFC, 0xB4, 0xD8, 0xFC, 0xB4, 0xC4, 0xFC, 0x00, 0x00, 0x70, 0x1C, 0x00, 0x70, 0x38, 0x00,
                0x70, 0x54, 0x00, 0x70, 0x70, 0x00, 0x70, 0x70, 0x00, 0x54, 0x70, 0x00, 0x38, 0x70, 0x00, 0x1C,
                0x70, 0x00, 0x00, 0x70, 0x1C, 0x00, 0x70, 0x38, 0x00, 0x70, 0x54, 0x00, 0x70, 0x70, 0x00, 0x54,
                0x70, 0x00, 0x38, 0x70, 0x00, 0x1C, 0x70, 0x00, 0x00, 0x70, 0x00, 0x00, 0x70, 0x1C, 0x00, 0x70,
                0x38, 0x00, 0x70, 0x54, 0x00, 0x70, 0x70, 0x00, 0x54, 0x70, 0x00, 0x38, 0x70, 0x00, 0x1C, 0x70,
                0x38, 0x38, 0x70, 0x44, 0x38, 0x70, 0x54, 0x38, 0x70, 0x60, 0x38, 0x70, 0x70, 0x38, 0x70, 0x70,
                0x38, 0x60, 0x70, 0x38, 0x54, 0x70, 0x38, 0x44, 0x70, 0x38, 0x38, 0x70, 0x44, 0x38, 0x70, 0x54,
                0x38, 0x70, 0x60, 0x38, 0x70, 0x70, 0x38, 0x60, 0x70, 0x38, 0x54, 0x70, 0x38, 0x44, 0x70, 0x38,
                0x38, 0x70, 0x38, 0x38, 0x70, 0x44, 0x38, 0x70, 0x54, 0x38, 0x70, 0x60, 0x38, 0x70, 0x70, 0x38,
                0x60, 0x70, 0x38, 0x54, 0x70, 0x38, 0x44, 0x70, 0x50, 0x50, 0x70, 0x58, 0x50, 0x70, 0x60, 0x50,
                0x70, 0x68, 0x50, 0x70, 0x70, 0x50, 0x70, 0x70, 0x50, 0x68, 0x70, 0x50, 0x60, 0x70, 0x50, 0x58,
                0x70, 0x50, 0x50, 0x70, 0x58, 0x50, 0x70, 0x60, 0x50, 0x70, 0x68, 0x50, 0x70, 0x70, 0x50, 0x68,
                0x70, 0x50, 0x60, 0x70, 0x50, 0x58, 0x70, 0x50, 0x50, 0x70, 0x50, 0x50, 0x70, 0x58, 0x50, 0x70,
                0x60, 0x50, 0x70, 0x68, 0x50, 0x70, 0x70, 0x50, 0x68, 0x70, 0x50, 0x60, 0x70, 0x50, 0x58, 0x70,
                0x00, 0x00, 0x40, 0x10, 0x00, 0x40, 0x20, 0x00, 0x40, 0x30, 0x00, 0x40, 0x40, 0x00, 0x40, 0x40,
                0x00, 0x30, 0x40, 0x00, 0x20, 0x40, 0x00, 0x10, 0x40, 0x00, 0x00, 0x40, 0x10, 0x00, 0x40, 0x20,
                0x00, 0x40, 0x30, 0x00, 0x40, 0x40, 0x00, 0x30, 0x40, 0x00, 0x20, 0x40, 0x00, 0x10, 0x40, 0x00,
                0x00, 0x40, 0x00, 0x00, 0x40, 0x10, 0x00, 0x40, 0x20, 0x00, 0x40, 0x30, 0x00, 0x40, 0x40, 0x00,
                0x30, 0x40, 0x00, 0x20, 0x40, 0x00, 0x10, 0x40, 0x20, 0x20, 0x40, 0x28, 0x20, 0x40, 0x30, 0x20,
                0x40, 0x38, 0x20, 0x40, 0x40, 0x20, 0x40, 0x40, 0x20, 0x38, 0x40, 0x20, 0x30, 0x40, 0x20, 0x28,
                0x40, 0x20, 0x20, 0x40, 0x28, 0x20, 0x40, 0x30, 0x20, 0x40, 0x38, 0x20, 0x40, 0x40, 0x20, 0x38,
                0x40, 0x20, 0x30, 0x40, 0x20, 0x28, 0x40, 0x20, 0x20, 0x40, 0x20, 0x20, 0x40, 0x28, 0x20, 0x40,
                0x30, 0x20, 0x40, 0x38, 0x20, 0x40, 0x40, 0x20, 0x38, 0x40, 0x20, 0x30, 0x40, 0x20, 0x28, 0x40,
                0x2C, 0x2C, 0x40, 0x30, 0x2C, 0x40, 0x34, 0x2C, 0x40, 0x3C, 0x2C, 0x40, 0x40, 0x2C, 0x40, 0x40,
                0x2C, 0x3C, 0x40, 0x2C, 0x34, 0x40, 0x2C, 0x30, 0x40, 0x2C, 0x2C, 0x40, 0x30, 0x2C, 0x40, 0x34,
                0x2C, 0x40, 0x3C, 0x2C, 0x40, 0x40, 0x2C, 0x3C, 0x40, 0x2C, 0x34, 0x40, 0x2C, 0x30, 0x40, 0x2C,
                0x2C, 0x40, 0x2C, 0x2C, 0x40, 0x30, 0x2C, 0x40, 0x34, 0x2C, 0x40, 0x3C, 0x2C, 0x40, 0x40, 0x2C,
                0x3C, 0x40, 0x2C, 0x34, 0x40, 0x2C, 0x30, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
        #endregion
    }
}
