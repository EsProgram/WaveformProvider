
<p align="center">
  <img src="https://github.com/EsProgram/WaveformProvider/blob/master/Image/logo.png" width="500"/>
</p>

------------


# WaveformProvider

Provide a texture to simulate waves with Unity.

<strong>Require InkPainter(nv1.2.1 or later).</strong>

> InkPainter Download
> * https://assetstore.unity.com/packages/tools/particles-effects/ink-painter-86210
> * https://github.com/EsProgram/InkPainter/releases

</br>
<p align="center">
   <img src="https://github.com/EsProgram/WaveformProvider/blob/master/Image/002.gif" width="600"/>
</p>
</br>

</br>
<p align="center">
   <img src="https://github.com/EsProgram/WaveformProvider/blob/master/Image/005.gif" width="600"/>
</p>
</br>












## Movies

<p align="center">
  <a href="https://www.youtube.com/watch?v=rpXeDkfSdwg">
   <img src="http://img.youtube.com/vi/rpXeDkfSdwg/0.jpg" width="600"/>
  </a>
</p>
<p align="center">
 WaveformProvider demo movie(Click image).
</p>
<br/>

</br>
<p align="center">
   <img src="https://github.com/EsProgram/WaveformProvider/blob/master/Image/001.gif" width="600"/>
</p>
</br>

</br>
<p align="center">
   <img src="https://github.com/EsProgram/WaveformProvider/blob/master/Image/003.gif" width="600"/>
</p>
</br>

</br>
<p align="center">
   <img src="https://github.com/EsProgram/WaveformProvider/blob/master/Image/004.gif" width="600"/>
</p>
</br>





© UTJ/UCL


## How to use

For details on scripts and shader libraries, please see the <a href="https://github.com/EsProgram/WaveformProvider/wiki/WaveformProvider">wiki</a>.

> https://github.com/EsProgram/WaveformProvider/wiki/WaveformProvider


### 1.Attach the WaveConductor component

Attach a WaveConductor component that inputs and outputs waveforms to an object.

</br>
<p align="center">
   <img src="https://github.com/EsProgram/WaveformProvider/blob/master/Image/setup001.png" width="600"/>
</p>
</br>

### 2.A set of RenderTexture for waveform output

Set RenderTexture to output waveform.
In the image, it is set from the inspector view, but it can acquire and set it from the script.

</br>
<p align="center">
   <img src="https://github.com/EsProgram/WaveformProvider/blob/master/Image/setup002.gif" width="600"/>
</p>
</br>

### 3.Setting of materials using ripples.

Create Shader that uses RenderTexture for waveform output set by Output of WaveConductor.And create a material using that shader and set it on the object.
In the image, use the Material of normal output of waves.

</br>
<p align="center">
   <img src="https://github.com/EsProgram/WaveformProvider/blob/master/Image/setup003.gif" width="600"/>
</p>
</br>

### 4.Attach a script to input waveforms.

Attach the script that inputs the waveform.
In the image, attach a script that inputs the waveform with mouse input.

</br>
<p align="center">
   <img src="https://github.com/EsProgram/WaveformProvider/blob/master/Image/setup004png.png" width="600"/>
</p>
</br>

### 5.Move and adjust.

Setup is complete.
After that, adjust each parameter.

</br>
<p align="center">
   <img src="https://github.com/EsProgram/WaveformProvider/blob/master/Image/setup005.gif" width="600"/>
</p>
</br>


# The MIT License

Copyright (c) 2018 Es_Program

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
