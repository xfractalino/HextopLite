# HextopLite
 
> A live wallpaper engine for Windows 11, rendering directly behind desktop icons via the Windows composition pipeline.
 
**Work in progress.** The core embedding mechanism is working — shaders and full rendering pipeline coming soon.
 
---
 
## How it works
 
Windows exposes an undocumented message (`0x052C`) that can be sent to `Progman` (the shell desktop window) to initialize a composited wallpaper surface.
On Windows 11 24H2+, Progman gains `WS_EX_NOREDIRECTIONBITMAP` and uses the Windows.UI.Composition pipeline exclusively, which changes how wallpaper embedding works compared to older Windows versions.
## Requirements
 
- Windows 11 24H2 or later (older versions untested but partially handled)
- .NET 10
- A GPU supporting Direct3D 11 feature level 11.0
## Dependencies
 
- [Vortice.Windows](https://github.com/amerkoleci/vortice.windows) — Direct3D 11 and DXGI bindings
- Windows SDK (via `net10.0-windows10.0.19041.0` TFM)
## Current state
 
- [x] Shell embedding behind desktop icons
- [x] Windows.UI.Composition visual tree on the wallpaper surface
- [ ] D3D11 rendering surface wired into the composition tree
- [ ] HLSL/GLSL shader pipeline
- [ ] Multi-monitor support
- [ ] Configuration and shader hot-reload
- [ ] Cleanup and teardown (send `0x052C` with `wParam=0xD, lParam=0x0` to restore Progman state)
