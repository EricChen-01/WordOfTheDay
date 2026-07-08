# Word of the Day

A full-stack app that serves a daily Japanese vocabulary word with reading, meaning, and usage. The application refreshes once a day for every visitor.

## Visit Live
https://wordoftheday.ericgchen.com

## What it does

Each day, a new Japanese word is generated and made available through an API. Visitors see the word for "today," along with its reading and meaning.

The word is the same for everyone globally, refreshing at UTC midnight. The frontend translates that into each visitor's local time so it's clear when the next word will appear.

## Why

I'm learning Japanese, and wanted a small daily habit to reinforce vocabulary.

## Structure

```
/backend    Azure Functions API — generates and serves the word of the day
/frontend   Vite + React app — displays the word of the day
```

Each folder has its own README with setup and run instructions.

## Status

Actively in development. Backend is functional; frontend is in progress.