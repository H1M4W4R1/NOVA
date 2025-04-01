# NOVA - Nonlinear Oscillation and Variable Adjustment
NOVA is a Waveform API used to create waveforms modulating variable (with method proxy). Can be helpful to easily create 
custom patterns for your software.

## Requirements
* C# 8.0

## Supported Waveforms
To check for supported waveforms see NOVA.Implementations directory.

## Example Usage
```csharp
// Create new sine frame
// 8Hz waveform with 0.5 amplitude and 0.25 offset
// It will modulate from 0.25 to 0.75 eight times per second
SineWaveform testWaveform = new(8, 0.5, 0.25); 

// Register method to update our variable, in this case it just logs message to console
testWaveform.OnWaveformValueChanged += PrintMessage;

// Start the waveform, it will automatically register
testWaveform.Start();

// ... Wait for some time

// Stop the waveform (if infinite)
testWaveform.Stop();
```

## Infinite and finite waveforms
Some waveforms are setting `Duration` in constructor, however you can also do this manually.
In such case when `Duration` ends waveform will be automatically stopped.

Setting `Duration` to any value below zero will result in infinite duration (looping).

```csharp
testWaveform.Duration = 1_000; // Set waveform duration to 1 second
```

## Running waveforms in sequence
```csharp
// Create sequence parts - ramp up and down, each for 1 second
RampWaveform rampUp = new(0, 1, 1000);
RampWaveform rampDown = new(1, 0, 1000);

// Create waveform sequence
SequentialWaveform waveform = new SequentialWaveform(false, rampUp, rampDown);
waveform.OnWaveformValueChanged += PrintMessage;

// Start the waveform sequence
waveform.Start();
```

## Supported events
* OnWaveformStart - performed right before waveforms begins
* OnWaveformEnd - performed after waveforms ends
* OnWaveformValueChanged - performed when waveform value changes
  * Is equal to current value if waveform is running
  * Is equal to default value right after waveform start and right before waveform end

## Waveform default value
`DefaultValue` is desired waveform value when it starts and ends (defaults to zero to disable waveform).