#include "Bannerlord.ModuleManager.h"
#include <napi.h>

using namespace Napi;

Object InitAll(const Env env, const Object exports)
{
  Bannerlord::ModuleManager::Init(env, exports);
  return exports;
}

NODE_API_MODULE(NODE_GYP_MODULE_NAME, InitAll)