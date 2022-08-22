#ifndef BLMANAGER_UTILS_GUARD 
#define BLMANAGER_UTILS_GUARD

#include <iomanip>
#include <sstream>
#include <napi.h>
#include "Bannerlord.ModuleManager.Native.h"

namespace Bannerlord {
    namespace ModuleManager {
        namespace Native {
            namespace Utils {

                const std::string escapeString(const std::string str) {
                    std::wstring_convert<std::codecvt_utf8<char16_t>, char16_t> conv;
                    const auto str16 = conv.from_bytes(str);

                    std::stringstream ss;
                    for (const auto c: str16) {
                        if (c > 127) {
                            ss << "\\u" << std::setw(4) << std::setfill('0') << std::hex << c;
                        }
                        else {
                            ss << (unsigned char) c;
                        }
                    }

                    return ss.str();
                }
                const char* unescapeString(const char* str) {
                    return str;
                }

                const Napi::String JSONStringify(const Napi::Env env, const Napi::Object object) {
                    const auto jsonObject = env.Global().Get("JSON").As<Napi::Object>();
                    const auto stringify = jsonObject.Get("stringify").As<Napi::Function>();
                    return stringify.Call(jsonObject, { object }).As<Napi::String>();
                }
                const Napi::Object JSONParse(const Napi::Env env, const Napi::String json) {
                    const auto jsonObject = env.Global().Get("JSON").As<Napi::Object>();
                    const auto parse = jsonObject.Get("parse").As<Napi::Function>();
                    return parse.Call(jsonObject, { json }).As<Napi::Object>();
                }

            }
        }
    }
}
#endif