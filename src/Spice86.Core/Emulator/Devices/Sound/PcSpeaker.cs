﻿namespace Spice86.Core.Emulator.Devices.Sound;

using Serilog;

using Spice86.Core.Emulator;

using Spice86.Core.Emulator.IOPorts;
using Spice86.Core.Emulator.Sound.PCSpeaker;
using Spice86.Core.Emulator.VM;
using Spice86.Core.Utils;
using Spice86.Logging;

/// <summary>
/// PC speaker implementation.
/// </summary>
public sealed class PcSpeaker : DefaultIOPortHandler, IDisposable {
    private static readonly ILogger _logger = Serilogger.Logger.ForContext<PcSpeaker>();
    private const int PcSpeakerPortNumber = 0x61;

    private bool _disposed = false;

    private readonly InternalSpeaker _pcSpeaker;

    public PcSpeaker(Machine machine, Configuration configuration) : base(machine, configuration) {
        _pcSpeaker = new(configuration);
    }

    public override byte ReadByte(int port) {
        byte value = _pcSpeaker.ReadByte(port);
        if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information)) {
            _logger.Information("PC Speaker get value {@PCSpeakerValue}", ConvertUtils.ToHex8(value));
        }
        return value;
    }

    public override void InitPortHandlers(IOPortDispatcher ioPortDispatcher) {
        ioPortDispatcher.AddIOPortHandler(PcSpeakerPortNumber, this);
    }

    public override void WriteByte(int port, byte value) {
        if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information)) {
            _logger.Information("PC Speaker set value {@PCSpeakerValue}", ConvertUtils.ToHex8(value));
        }

        _pcSpeaker.WriteByte(port, value);
    }

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
        if(!_disposed){
            if(disposing) {
                _pcSpeaker.Dispose();
            }
            _disposed = true;
        }
    }
}