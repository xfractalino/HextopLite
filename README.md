# HextopLite
 
> A live wallpaper engine for Windows 11, rendering directly behind desktop icons via the Windows composition pipeline.
 
**Work in progress.** The core embedding mechanism is working — shaders and full rendering pipeline coming soon.
 
---
 
## How it works
 
Windows exposes an undocumented message (`0x052C`) that can be sent to `Progman` (the shell desktop window) to initialize a composited wallpaper surface.
On Windows 11 24H2+, Progman gains `WS_EX_NOREDIRECTIONBITMAP` and uses the Windows.UI.Composition pipeline exclusively, which changes how wallpaper embedding works compared to older Windows versions.
 
## How it differs from previous implementations
 
Most documentation and prior art for the WorkerW trick targets older Windows versions where the hierarchy is:
 
```
Progman
└── WorkerW
    └── SHELLDLL_DefView
        └── SysListView32 (icons)
```
 
On Windows 11 24H2+, the hierarchy changed:
 
```
Progman  (WS_EX_NOREDIRECTIONBITMAP)
├── SHELLDLL_DefView
│   └── SysListView32 (icons)
└── WorkerW
```
 
This has several consequences:
 
- The correct embed parent is **Progman**, not WorkerW
- Cross-process D3D swap chain presentation does not survive `SetParent` under DWM
- GDI writes to foreign window DCs are ignored by the composition pipeline
- Only `WS_EX_NOREDIRECTIONBITMAP` + Windows.UI.Composition (`DesktopWindowTarget`) survives reparenting correctly
- The WinRT `Compositor` requires a `DispatcherQueue` on the rendering thread and an STA apartment
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
