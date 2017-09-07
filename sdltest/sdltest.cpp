#include <SDL.h>

int main(int argc, char const *argv[])
{
    SDL_Event event;

    SDL_SetHint(SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS, "1");
    SDL_Init( SDL_INIT_VIDEO | SDL_INIT_JOYSTICK | SDL_INIT_GAMECONTROLLER);
    atexit(SDL_Quit);

	SDL_GameController *controller = NULL;
	for (int i = 0; i < SDL_NumJoysticks(); ++i) {
	    if (SDL_IsGameController(i)) {
	        controller = SDL_GameControllerOpen(i);
	        if (controller) {
                printf("%s\n", SDL_GameControllerName(controller));
	            break;
	        } else {
	            fprintf(stderr, "Could not open gamecontroller %i: %s\n", i, SDL_GetError());
	        }
	    }
	}

    while ( SDL_WaitEvent(&event) >= 0 ) {
        switch (event.type) {
            case SDL_CONTROLLERAXISMOTION: {
                printf("Axis motion axis %d - value: %d\n", event.jaxis.axis, event.jaxis.value);
            }
                break;

            case SDL_CONTROLLERBUTTONDOWN: {
                printf("Button %d down\n", event.jbutton.button);
            }
                break;

            case SDL_QUIT: {
                printf("Quit requested, quitting.\n");
                exit(0);
            }
        }
    }
	return 0;
}
