#!/bin/bash

# Deployment script for Cao Gia Construction
# Usage: ./deploy.sh [tag]

set -e

IMAGE_TAG=${1:-latest}
DOCKER_IMAGE="ngocsonit95/cao-gia-construction"
COMPOSE_FILE="docker-compose.yml"

echo "🚀 Starting deployment with tag: $IMAGE_TAG"

# Check if docker-compose.yml exists
if [ ! -f "$COMPOSE_FILE" ]; then
    echo "❌ Error: $COMPOSE_FILE not found"
    exit 1
fi

# Pull latest image
echo "📥 Pulling latest image..."
docker pull ${DOCKER_IMAGE}:${IMAGE_TAG}

# Update docker-compose with new tag
export IMAGE_TAG=${IMAGE_TAG}

# Stop and remove old containers
echo "🛑 Stopping old containers..."
docker-compose down

# Start new containers
echo "▶️  Starting new containers..."
docker-compose up -d

# Clean up old images
echo "🧹 Cleaning up old images..."
docker system prune -f

echo "✅ Deployment completed successfully!"
echo "📊 Container status:"
docker-compose ps

