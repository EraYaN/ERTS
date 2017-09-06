
CC=gcc
CFLAGS = -g -Wall -lm
EXEC = ./erts-pc.out

all:
	$(CC) $(CFLAGS) erts-pc.c -o $(EXEC)

run: all
	$(EXEC)