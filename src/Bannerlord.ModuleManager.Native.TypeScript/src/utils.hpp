#ifndef BMM_UTILS_GUARD_HPP_
#define BMM_UTILS_GUARD_HPP_

#include <Common.Native.h>
#include <napi.h>
#include <cstdint>

using namespace Napi;
using namespace Common;

namespace Utils
{

    void ConsoleLog(const Env env, const String message)
    {
        const auto consoleObject = env.Global().Get("console").As<Object>();
        const auto log = consoleObject.Get("log").As<Function>();
        log.Call(consoleObject, {message});
    }

    const String JSONStringify(const Env env, const Object object)
    {
        const auto jsonObject = env.Global().Get("JSON").As<Object>();
        const auto stringify = jsonObject.Get("stringify").As<Function>();
        return stringify.Call(jsonObject, {object}).As<String>();
    }
    const Object JSONParse(const Env env, const String json)
    {
        const auto jsonObject = env.Global().Get("JSON").As<Object>();
        const auto parse = jsonObject.Get("parse").As<Function>();
        return parse.Call(jsonObject, {json}).As<Object>();
    }

    void ThrowOrReturn(Env env, return_value_void *val)
    {
        const del_void del{val};

        if (val->error == nullptr)
        {
            return;
        }
        const auto error = std::unique_ptr<char16_t[], deleter<char16_t>>(val->error);
        NAPI_THROW(Error::New(env, String::New(env, error.get())));
    }
    const Value ThrowOrReturnString(Env env, return_value_string *val)
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
    const Value ThrowOrReturnJson(Env env, return_value_json *val)
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
    const Value ThrowOrReturnBoolean(Env env, return_value_bool *val)
    {
        const del_bool del{val};

        if (val->error == nullptr)
        {
            return Boolean::New(env, val->value);
        }
        const auto error = std::unique_ptr<char16_t[], deleter<char16_t>>(val->error);
        NAPI_THROW(Error::New(env, String::New(env, error.get())));
    }
    const Value ThrowOrReturnInt32(Env env, return_value_int32 *val)
    {
        const del_int32 del{val};

        if (val->error == nullptr)
        {
            return Number::New(env, val->value);
        }
        const auto error = std::unique_ptr<char16_t[], deleter<char16_t>>(val->error);
        NAPI_THROW(Error::New(env, String::New(env, error.get())));
    }
    const Value ThrowOrReturnUInt32(Env env, return_value_uint32 *val)
    {
        const del_uint32 del{val};

        if (val->error == nullptr)
        {
            return Number::New(env, val->value);
        }
        const auto error = std::unique_ptr<char16_t[], deleter<char16_t>>(val->error);
        NAPI_THROW(Error::New(env, String::New(env, error.get())));
    }
    void *ThrowOrReturnPtr(Env env, return_value_ptr *val)
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
