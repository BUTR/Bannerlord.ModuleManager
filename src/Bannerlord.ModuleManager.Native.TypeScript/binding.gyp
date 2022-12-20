{
    "targets": [
        {
            "target_name": "blmanager",
            "sources": [
                "<(module_root_dir)/src/main.cpp",
            ],
            'include_dirs': [
                "<!@(node -p \"require('node-addon-api').include\")",
                "<(module_root_dir)"
            ],
            'libraries': [
                "<(module_root_dir)/Bannerlord.ModuleManager.Native.lib"
            ],
            'dependencies': [
                "<!(node -p \"require('node-addon-api').gyp\")"
            ],
            'defines': [ 
                'NAPI_CPP_EXCEPTIONS',
                '_SILENCE_CXX17_CODECVT_HEADER_DEPRECATION_WARNING',

            ],
            'msvs_settings': {
              'VCCLCompilerTool': {
                  'AdditionalOptions': [
                      '/EHsc',
                      '/std:c++17',
                  ],
                  'ExceptionHandling': 1,
                  'EnablePREfast': 'true',
              }
            },
        }
    ]
}