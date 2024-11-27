#!/bin/bash

# Name of the tmux session
SESSION="dev"

# Start a new tmux session (or attach if it exists)
tmux has-session -t $SESSION 2>/dev/null


if [ $? != 0 ]; then
    npm run watch:docker:detach 
    echo "Starting a new tmux session: $SESSION"
    tmux new-session -d -s $SESSION

    # Create panes and run commands
#    tmux send-keys -t $SESSION "npm run watch:docker:detach " C-m
    tmux send-keys -t $SESSION "docker-compose attach studentescu" C-m
    tmux split-window -h -t $SESSION
    tmux send-keys -t $SESSION "docker-compose attach mysql" C-m
    tmux split-window -v -t $SESSION
    tmux send-keys -t $SESSION "npm run watch:dev" C-m

    # Adjust pane layout
    tmux select-layout -t $SESSION tiled
fi

# Attach to the session
tmux attach -t $SESSION
