# RabbitMQ Installation Guide

Quick installation guide for RabbitMQ. Choose either Docker (recommended) or Windows Installer.

---

## Option 1: Docker (Recommended)

### Prerequisites
- Docker Desktop installed and running

### Installation
Run this single command:
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### Verify
- **Management UI**: http://localhost:15672
- **Login**: username `guest`, password `guest`
- Should see RabbitMQ dashboard

### Useful Commands
```bash
# Stop RabbitMQ
docker stop rabbitmq

# Start RabbitMQ
docker start rabbitmq

# View logs
docker logs rabbitmq

# Remove container
docker rm -f rabbitmq
```

---

## Option 2: Windows Installer

### Step 1: Install Erlang
RabbitMQ runs on Erlang VM, so install it first.

1. Download Erlang: https://www.erlang.org/downloads
2. Get **OTP 26.x** (latest stable)
3. Run installer with default options
4. Verify (optional): In PowerShell run `$env:Path += ";C:\Program Files\Erlang OTP\bin"` then `erl -version`

### Step 2: Install RabbitMQ
1. Download RabbitMQ: https://www.rabbitmq.com/install-windows.html
2. Get **Windows Installer (.exe)**
3. Run installer with default options
4. RabbitMQ installs as Windows Service (auto-starts)

### Step 3: Enable Management Plugin
Open **PowerShell as Administrator**:
```powershell
cd "C:\Program Files\RabbitMQ Server\rabbitmq_server-3.13.x\sbin"
.\rabbitmq-plugins.bat enable rabbitmq_management
```

*Note: Replace `3.13.x` with your actual version number*

### Step 4: Verify Installation
```powershell
# Check RabbitMQ status
.\rabbitmqctl.bat status

# Or check Windows Services
services.msc
# Look for "RabbitMQ" service (should be Running)
```

**If status command fails with cookie error**, run in PowerShell as Administrator:
```powershell
Stop-Service RabbitMQ
Copy-Item "C:\Windows\System32\config\systemprofile\.erlang.cookie" "$env:USERPROFILE\.erlang.cookie" -Force
Start-Service RabbitMQ
Start-Sleep -Seconds 5
.\rabbitmqctl.bat status
```

### Access Management UI
- **URL**: http://localhost:15672
- **Login**: username `guest`, password `guest`
- Should see RabbitMQ dashboard

### Useful Commands (in RabbitMQ sbin folder)
```powershell
# Start service
.\rabbitmq-service.bat start

# Stop service
.\rabbitmq-service.bat stop

# Restart service
.\rabbitmq-service.bat restart
```

---

## Verify Installation (Both Options)

### 1. Check Management UI
- Open http://localhost:15672
- Login with `guest` / `guest`
- You should see the RabbitMQ dashboard

### 2. Check Ports
```bash
# Port 5672: AMQP protocol (application connections)
# Port 15672: Management UI (web interface)
```

### 3. Create Test Queue (Optional)
In Management UI:
1. Go to **Queues** tab
2. Click **Add a new queue**
3. Name: `test-queue`
4. Click **Add queue**
5. Should appear in list

---

## Troubleshooting

### Docker: Container won't start
```bash
# Check if ports are already in use
netstat -ano | findstr :5672
netstat -ano | findstr :15672

# Use different ports if needed
docker run -d --name rabbitmq -p 5673:5672 -p 15673:15672 rabbitmq:3-management
```

### Windows: Service won't start
1. Check Windows Event Viewer for errors
2. Verify Erlang is installed: `erl -version`
3. Reinstall RabbitMQ with "Run as Administrator"

### Management UI not accessible
```powershell
# Ensure plugin is enabled
.\rabbitmq-plugins.bat list
# Should see "[E*] rabbitmq_management"

# If not enabled
.\rabbitmq-plugins.bat enable rabbitmq_management
.\rabbitmq-service.bat restart
```

### Cannot login to Management UI
- Default credentials: `guest` / `guest`
- Guest user only works from `localhost`
- For remote access, create new user via `rabbitmqctl`

---

## Next Steps

Once RabbitMQ is installed and running:
1. Verify Management UI is accessible
2. Return to [MESSAGE_QUEUE_PLAN.md](MESSAGE_QUEUE_PLAN.md)
3. Start with Phase 1: Code implementation
