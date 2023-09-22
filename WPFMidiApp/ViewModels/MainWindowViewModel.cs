using DevExpress.Mvvm.CodeGenerators.Prism;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;

namespace WPFMidiApp.ViewModels
{
    [GenerateViewModel]
    public partial class MainWindowViewModel
    {
        #region Properties
        [GenerateProperty]
        private string title = "WPF Midi App";
        [GenerateProperty]
        private string messages = string.Empty;

        public ObservableCollection<DeviceInformation> InputDevices { get; set; } = new ObservableCollection<DeviceInformation>();
        public ObservableCollection<DeviceInformation> OutputDevices { get; set; } = new ObservableCollection<DeviceInformation>();
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {

        } 
        #endregion

        [GenerateCommand]
        public async Task Loaded()
        {
            // Find all input MIDI devices
            var midiInputQueryString = MidiInPort.GetDeviceSelector();
            var midiInputDevices = await DeviceInformation.FindAllAsync(midiInputQueryString);

            // Find all output MIDI devices
            var midiOutportQueryString = MidiOutPort.GetDeviceSelector();
            var midiOutputDevices = await DeviceInformation.FindAllAsync(midiOutportQueryString);

            //Add all input midi devices to ObservableCollection + subscribe to MessageReceived event
            foreach (var inputDev in midiInputDevices)
            {
                await Application.Current.Dispatcher.BeginInvoke(() => InputDevices.Add(inputDev));

                var midiInPort = await MidiInPort.FromIdAsync(inputDev.Id);
                midiInPort.MessageReceived += async (_, e) =>
                {
                    Debug.WriteLine(e.Message.Type.ToString());

                    if (e.Message.Type == MidiMessageType.NoteOn)
                    {
                        var message = e.Message as MidiNoteOnMessage;

                        await Application.Current.Dispatcher.BeginInvoke(() => Messages += message.Timestamp + 
                                                                                           ": [" + 
                                                                                           message.Type.ToString() +
                                                                                           " | " +
                                                                                           message.Note.ToString() +
                                                                                           " | " +
                                                                                           message.Velocity.ToString() +
                                                                                           "]" +
                                                                                           Environment.NewLine);
                    }
                    else if (e.Message.Type == MidiMessageType.NoteOff)
                    {
                        var message = e.Message as MidiNoteOffMessage;

                        await Application.Current.Dispatcher.BeginInvoke(() => Messages += message.Timestamp +
                                                                                           ": [" +
                                                                                           message.Type.ToString() +
                                                                                           " | " +
                                                                                           message.Note.ToString() +
                                                                                           " | " +
                                                                                           message.Velocity.ToString() +
                                                                                           "]" +
                                                                                           Environment.NewLine);
                    } 
                    else if (e.Message.Type == MidiMessageType.ControlChange)
                    {
                        var message = e.Message as MidiControlChangeMessage;

                        await Application.Current.Dispatcher.BeginInvoke(() => Messages += message.Timestamp +
                                                                                           ": [" +
                                                                                           message.Type.ToString() +
                                                                                           " | " +
                                                                                           message.Controller.ToString() +
                                                                                           " | " +
                                                                                           message.ControlValue.ToString() +
                                                                                           "]" +
                                                                                           Environment.NewLine);
                    }
                };
            }

            //Add all output MIDI devices to ObservableCollection
            foreach (var outputDev in midiOutputDevices)
            {
                await Application.Current.Dispatcher.BeginInvoke(() => OutputDevices.Add(outputDev));
            } 
        }
    }
}
