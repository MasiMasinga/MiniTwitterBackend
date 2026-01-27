# Mini Tweeter (Twitter Clone)

## Description

This is a backend application developed using .Net Web API. It serves as the API for a social media application that is a clone of Twitter. The application supports user authentication, and allows users to create, read, update, and delete (CRUD) tweets. Users can also comment on tweets and like them.

## API Endpoint

https://minitwitterbackend-aznfhq.fly.dev/

## Installation

Ensure you have Docker and Docker Compose installed on your machine.

1. Clone the repository:

    ```
    git clone  ''
    cd repository
    ```

2. Build and run the Docker containers:

    ```
    docker-compose up --build
    ```

The application should now be running at `http://localhost:5145`.

## API Endpoints

### Authentication Endpoints

- `POST /Api/Auth/Register`: Register a new user.
- `POST /Api/Auth/Login`: Login a user.
- `GET /Api/Auth/Logout`: Logout a user.

### Tweets Endpoints

- `POST /Api/Tweet`: Create a new tweet.
- `GET /Api/Tweets`: Fetch all tweets.
- `GET /Api/Tweet/<id>`: Fetch a specific tweet.
- `PUT /Api/Tweet/<id>`: Update a specific tweet.
- `DELETE /Api/Tweet/<id>`: Delete a specific tweet.
- `POST /api/Tweet/<id>/Like`: Like a specific tweet.

### Comments Endpoints

- `POST /api/Comment/<tweet_id>/Tweet`: Add a comment to a specific tweet.
- `GET /api/Comments/<tweet_id>/Tweet`: Get all comments for a specific tweet.
- `POST /api/Comment/<tweet_id>/Like`: Like a Comment on tweet.
- `PUT /api/Comment/<tweet_id>/Unlike`: Unlike a Comment on tweet.

### Retweet Endpoints

- `POST /api/Retweet/<comment_id>/Comment`: Retweet a Comment.
- `POST /api/Retweet/<tweet_id>/Tweet`: Retweet a Tweet.

### Make Payment Endpoints

- `POST /api/Payment`: Make Payment to Increase Tweets Quota.
- `GET /api/Payment`: Verify Payment to Check Status.

## Running the Application Without Docker


## Entity Models

![Mini Tweeter](MiniTweeter.png)

