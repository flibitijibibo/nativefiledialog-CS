/* nativefiledialog# - C# Wrapper for nativefiledialog
 *
 * Copyright (c) 2021 Ethan Lee.
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. If you use this software in a
 * product, an acknowledgment in the product documentation would be
 * appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not be
 * misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source distribution.
 *
 * Ethan "flibitijibibo" Lee <flibitijibibo@flibitijibibo.com>
 *
 */

#include <SDL.h>
#include "nfd.h"

static void* backend = NULL;

static SDL_bool NFD_INTERNAL_LoadBackend(void)
{
	static const char *backends[] =
	{
		"libnfd_gtk.so",
		"libnfd_zenity.so"
	};
	Uint8 i;
	for (i = 0; i < SDL_arraysize(backends); i += 1)
	{
		backend = SDL_LoadObject(backends[i]);
		if (backend != NULL)
		{
			return SDL_TRUE;
		}
	}
	return SDL_FALSE;
}

nfdresult_t NFD_OpenDialog( const nfdchar_t *filterList,
                            const nfdchar_t *defaultPath,
                            nfdchar_t **outPath )
{
	if (backend == NULL)
	{
		if (!NFD_INTERNAL_LoadBackend())
		{
			return NFD_ERROR;
		}
	}

	nfdresult_t (*backend_OpenDialog)(
		const nfdchar_t *filterList,
		const nfdchar_t *defaultPath,
		nfdchar_t **outPath
	) = SDL_LoadFunction(backend, "NFD_OpenDialog");
	return backend_OpenDialog(filterList, defaultPath, outPath);
}

nfdresult_t NFD_OpenDialogMultiple( const nfdchar_t *filterList,
                                    const nfdchar_t *defaultPath,
                                    nfdpathset_t *outPaths )
{
	if (backend == NULL)
	{
		if (!NFD_INTERNAL_LoadBackend())
		{
			return NFD_ERROR;
		}
	}

	nfdresult_t (*backend_OpenDialogMultiple)(
		const nfdchar_t *filterList,
		const nfdchar_t *defaultPath,
		nfdpathset_t *outPaths
	) = SDL_LoadFunction(backend, "NFD_OpenDialogMultiple");
	return backend_OpenDialogMultiple(filterList, defaultPath, outPaths);
}

nfdresult_t NFD_SaveDialog( const nfdchar_t *filterList,
                            const nfdchar_t *defaultPath,
                            nfdchar_t **outPath )
{
	if (backend == NULL)
	{
		if (!NFD_INTERNAL_LoadBackend())
		{
			return NFD_ERROR;
		}
	}

	nfdresult_t (*backend_SaveDialog)(
		const nfdchar_t *filterList,
		const nfdchar_t *defaultPath,
		nfdchar_t **outPath
	) = SDL_LoadFunction(backend, "NFD_SaveDialog");
	return backend_SaveDialog(filterList, defaultPath, outPath);
}

nfdresult_t NFD_PickFolder( const nfdchar_t *defaultPath,
                            nfdchar_t **outPath)
{
	if (backend == NULL)
	{
		if (!NFD_INTERNAL_LoadBackend())
		{
			return NFD_ERROR;
		}
	}

	nfdresult_t (*backend_PickFolder)(
		const nfdchar_t *defaultPath,
		nfdchar_t **outPath
	) = SDL_LoadFunction(backend, "NFD_PickFolder");
	return backend_PickFolder(defaultPath, outPath);
}

const char *NFD_GetError( void )
{
	if (backend == NULL)
	{
		return "No NFD backend has been loaded!";
	}

	const char *(*backend_GetError)(void) = SDL_LoadFunction(backend, "NFD_GetError");
	return backend_GetError();
}

size_t NFD_PathSet_GetCount( const nfdpathset_t *pathset )
{
	SDL_assert(pathset);
	return pathset->count;
}

nfdchar_t *NFD_PathSet_GetPath( const nfdpathset_t *pathset, size_t num )
{
	SDL_assert(pathset);
	SDL_assert(num < pathset->count);

	return pathset->buf + pathset->indices[num];
}

void NFD_PathSet_Free( nfdpathset_t *pathset )
{
	SDL_assert(pathset);
	SDL_assert(pathset->indices);
	SDL_assert(pathset->buf);
	free( pathset->indices );
	free( pathset->buf );
}
