#ifndef BMM_UTILS_GUARD_HPP_
#define BMM_UTILS_GUARD_HPP_

#include <napi.h>
#include <cstdint>
#include <Bannerlord.ModuleManager.Native.h>

using namespace Napi;
using namespace Bannerlord::ModuleManager::Native;

namespace Utils
{

    template <typename T>
    struct deleter
    {
        void operator()(T *const ptr) const { common_dealloc(static_cast<void *const>(ptr)); }
    };

    using del_void = std::unique_ptr<return_value_void, deleter<return_value_void>>;
    using del_string = std::unique_ptr<return_value_string, deleter<return_value_string>>;
    using del_json = std::unique_ptr<return_value_json, deleter<return_value_json>>;
    using del_bool = std::unique_ptr<return_value_bool, deleter<return_value_bool>>;
    using del_int32 = std::unique_ptr<return_value_int32, deleter<return_value_int32>>;
    using del_uint32 = std::unique_ptr<return_value_uint32, deleter<return_value_uint32>>;
    using del_ptr = std::unique_ptr<return_value_ptr, deleter<return_value_ptr>>;

    char16_t *const Copy(const std::u16string str)
    {
        const auto src = str.c_str();
        const auto srcChar16Length = str.length();
        const auto srcByteLength = srcChar16Length * sizeof(char16_t);
        const auto size = srcByteLength + sizeof(char16_t);

        auto dst = static_cast<char16_t *const>(common_alloc(size));
        if (dst == nullptr)
        {
            throw std::bad_alloc();
        }
        std::memmove(dst, src, srcByteLength);
        dst[srcChar16Length] = '\0';
        return dst;
    }

    std::unique_ptr<char16_t[], deleter<char16_t>> CopyWithFree(const std::u16string str)
    {
        return std::unique_ptr<char16_t[], deleter<char16_t>>(Copy(str));
    }

    const char16_t *const NoCopy(const std::u16string str) noexcept
    {
        return str.c_str();
    }

    template <typename T>
    T *const Create(const T val)
    {
        const auto size = sizeof(T);
        auto dst = static_cast<T *const>(common_alloc(size));
        if (dst == nullptr)
        {
            throw std::bad_alloc();
        }
        std::memcpy(dst, &val, sizeof(T));
        return dst;
    }

    void ConsoleLog(const Env env, const String message)
    {
        const auto consoleObject = env.Global().Get("console").As<Object>();
        const auto log = consoleObject.Get("log").As<Function>();
        log.Call(consoleObject, {message});
    }

    String JSONStringify(const Env env, const Object object)
    {
        const auto jsonObject = env.Global().Get("JSON").As<Object>();
        const auto stringify = jsonObject.Get("stringify").As<Function>();
        return stringify.Call(jsonObject, {object}).As<String>();
    }
    Object JSONParse(const Env env, const String json)
    {
        const auto jsonObject = env.Global().Get("JSON").As<Object>();
        const auto parse = jsonObject.Get("parse").As<Function>();
        return parse.Call(jsonObject, {json}).As<Object>();
    }

    void ThrowOrReturn(const Env env, return_value_void *const val)
    {
        const del_void del{val};

        if (val->error == nullptr)
        {
            return;
        }
        const auto error = std::unique_ptr<char16_t[], deleter<char16_t>>(val->error);
        NAPI_THROW(Error::New(env, String::New(env, error.get())));
    }
    Value ThrowOrReturnString(const Env env, return_value_string *const val)
    {
        const del_string del{val};

        if (val->error == nullptr)
        {
            if (val->value == nullptr)
            {
                NAPI_THROW(Error::New(env, String::New(env, "Return value was null!")));
            }

            const auto value = std::unique_ptr<char16_t[], deleter<char16_t>>(val->value);
            return String::New(env, val->value);
        }
        const auto error = std::unique_ptr<char16_t[], deleter<char16_t>>(val->error);
        NAPI_THROW(Error::New(env, String::New(env, error.get())));
    }
    Value ThrowOrReturnJson(const Env env, return_value_json *const val)
    {
        const del_json del{val};

        if (val->error == nullptr)
        {
            if (val->value == nullptr)
            {
                NAPI_THROW(Error::New(env, String::New(env, "Return value was null!")));
            }

            const auto value = std::unique_ptr<char16_t[], deleter<char16_t>>(val->value);
            return JSONParse(env, String::New(env, val->value));
        }
        const auto error = std::unique_ptr<char16_t[], deleter<char16_t>>(val->error);
        NAPI_THROW(Error::New(env, String::New(env, error.get())));
    }
    Value ThrowOrReturnBoolean(const Env env, return_value_bool *const val)
    {
        const del_bool del{val};

        if (val->error == nullptr)
        {
            return Boolean::New(env, val->value);
        }
        const auto error = std::unique_ptr<char16_t[], deleter<char16_t>>(val->error);
        NAPI_THROW(Error::New(env, String::New(env, error.get())));
    }
    Value ThrowOrReturnInt32(const Env env, return_value_int32 *const val)
    {
        const del_int32 del{val};

        if (val->error == nullptr)
        {
            return Number::New(env, val->value);
        }
        const auto error = std::unique_ptr<char16_t[], deleter<char16_t>>(val->error);
        NAPI_THROW(Error::New(env, String::New(env, error.get())));
    }
    Value ThrowOrReturnUInt32(const Env env, return_value_uint32 *const val)
    {
        const del_uint32 del{val};

        if (val->error == nullptr)
        {
            return Number::New(env, val->value);
        }
        const auto error = std::unique_ptr<char16_t[], deleter<char16_t>>(val->error);
        NAPI_THROW(Error::New(env, String::New(env, error.get())));
    }
    void *ThrowOrReturnPtr(const Env env, return_value_ptr *const val)
    {
        const del_ptr del{val};

        if (val->error == nullptr)
        {
            return val->value;
        }
        const auto error = std::unique_ptr<char16_t[], deleter<char16_t>>(val->error);
        NAPI_THROW(Error::New(env, String::New(env, error.get())));
    }

}

#endif
