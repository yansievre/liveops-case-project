This folder will be compiled only for editor builds. 
It contains scripts that are used by both the editor and runtime, but are not needed in the final build. 
This is useful for editor tools, debug scripts, and other utilities that are not needed in the final build.
Any content of this folder that is accessed by the runtime should be wrapped in #if UNITY_EDITOR to prevent it from being included in the final build.