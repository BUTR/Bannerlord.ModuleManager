#ifndef BFE_COMMON_GUARD_HPP_
#define BFE_COMMON_GUARD_HPP_

#include <napi.h>
#include <Bannerlord.ModuleManager.Native.h>

using namespace Napi;
using namespace Bannerlord::ModuleManager::Native;

namespace Bannerlord::Common
{

    Value AllocAliveCount(const CallbackInfo &info)
    {
        const auto env = info.Env();

        const auto result = common_alloc_alive_count();
        return Number::New(env, result);
    }

    Object Init(const Env env, const Object exports)
    {
        exports.Set("allocAliveCount", Function::New(env, AllocAliveCount));

        return exports;
    }

}
#endif