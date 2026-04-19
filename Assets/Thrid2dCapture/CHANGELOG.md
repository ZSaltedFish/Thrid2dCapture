# Changelog

All notable changes to this project will be documented in this file.

## [1.2.1] - 2026-04-19
### Added
- **CustomGen 功能**: 在自动生成 AnimatorController 时支持自定义逻辑调整，提升了工作流的灵活性。

## [1.2.0] - 2026-03-15
### Added
- **TextureArray 渲染支持**: 将所有朝向的动画帧整合进单个 TextureArray。
- **Shader 索引优化**: 实现了基于 `朝向索引 * 动作帧数 + 帧号` 的高效 Shader 采样逻辑。
### Changed
- 废弃了原有的“单动作多状态机”驱动模式，大幅降低了 Draw Call。

## [1.1.0] - 2025-12-20
### Fixed
- 修复了 16 向相机旋转时的微小角度偏移问题。
