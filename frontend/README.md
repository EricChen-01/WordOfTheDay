# Word of the Day

A daily Japanese vocabulary app. A backend job pre-generates a word each day and stores it; the frontend fetches and displays it.

## Visit live site
https://wordoftheday.ericgchen.com

## Running locally

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Create a `.env.development` file in `/frontend` with:

```
VITE_API_URL=http://localhost:7071
```

(Port depends on whatever `func start` binds to — check the terminal output.)

## Deployment

Make changes to the front end and merge into main will result in auto deployment. 