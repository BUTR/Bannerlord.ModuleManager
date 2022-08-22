{
    "targets": [
        {
            "target_name": "blmanager",
            "cflags!": [ "-fno-exceptions" ],
            "cflags_cc!": [ "-fno-exceptions" ],
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
            'defines': [ 'NAPI_DISABLE_CPP_EXCEPTIONS' ],
			'msvs_settings': {
				'VCCLCompilerTool': {
					'AdditionalOptions': [
                        '/EHsc',
					],
                    'ExceptionHandling': 0,
                    'EnablePREfast': 'true',
				}
			},
        }
    ]
}