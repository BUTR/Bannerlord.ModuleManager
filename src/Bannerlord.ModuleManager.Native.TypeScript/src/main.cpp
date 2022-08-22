#include <napi.h>
#include "wrapper.h"

using namespace Napi;
using namespace Bannerlord::ModuleManager::Native::Wrapper;

Object InitAll(const Env env, const Object exports) {
  return Init(env, exports);
}

NODE_API_MODULE(NODE_GYP_MODULE_NAME, InitAll)