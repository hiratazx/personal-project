import re
from datetime import datetime, timedelta

def parse_input_string(input_str):
    """
    Separates the number from the unit (e.g., '90GB' -> 90.0, 'GB').
    """
    input_str = input_str.strip()
    match = re.match(r"(\d+(?:\.\d+)?)\s*([a-zA-Z/]+)", input_str)
    
    if not match:
        raise ValueError(f"Could not understand format: {input_str}")
    
    val = float(match.group(1))
    unit = match.group(2)
    return val, unit

def convert_size_to_bits(size_val, unit):
    """
    Converts file size to bits. 
    Assumes standard file system binary prefixes (1 KB = 1024 Bytes).
    """
    unit = unit.upper()
    
    base = 8 
    
    if 'TB' in unit:
        return size_val * (1024**4) * base
    elif 'GB' in unit:
        return size_val * (1024**3) * base
    elif 'MB' in unit:
        return size_val * (1024**2) * base
    elif 'KB' in unit:
        return size_val * 1024 * base
    elif 'B' in unit: # Bytes
        return size_val * base
    else:
        return size_val

def convert_speed_to_bits_per_sec(speed_val, unit):
    """
    Converts speed to bits per second (bps).
    Distinguishes between Mbps (decimal bits) and MB/s (binary Bytes).
    """
    
    clean_unit = unit.lower()
    
    if 'b/s' in clean_unit or 'byte' in clean_unit:
        if 't' in clean_unit: return speed_val * (1024**4) * 8
        if 'g' in clean_unit: return speed_val * (1024**3) * 8
        if 'm' in clean_unit: return speed_val * (1024**2) * 8
        if 'k' in clean_unit: return speed_val * 1024 * 8
        return speed_val * 8 # Bytes/s

    else: 
        if 't' in clean_unit: return speed_val * (1000**4)
        if 'g' in clean_unit: return speed_val * (1000**3)
        if 'm' in clean_unit: return speed_val * (1000**2)
        if 'k' in clean_unit: return speed_val * 1000
        return speed_val # bps

def format_duration(seconds):
    """Formats seconds into readable H:M:S string."""
    seconds = int(round(seconds))
    d = timedelta(seconds=seconds)
    return str(d)

def main():
    print("Examples: Size: 90GB, 500MB | Speed: 200Mbps, 15MB/s")
    print("-" * 35)

    try:
        size_input = input("Enter File Size (e.g., 90GB): ").strip()
        size_val, size_unit = parse_input_string(size_input)
        total_bits = convert_size_to_bits(size_val, size_unit)

        speed_input = input("Enter Download Speed (e.g., 200Mbps or 25MB/s): ").strip()
        speed_val, speed_unit = parse_input_string(speed_input)
        speed_bps = convert_speed_to_bits_per_sec(speed_val, speed_unit)

        if speed_bps == 0:
            print("Speed cannot be zero.")
            return

        total_seconds = total_bits / speed_bps
        formatted_duration = format_duration(total_seconds)

        print("\n" + "="*30)
        print(f"Total Duration: {formatted_duration} (Hours:Min:Sec)")

        start_time_str = input("\nEnter Start Time (e.g., 10:00 AM) or press Enter to skip: ").strip()

        if start_time_str:
            formats = ["%I:%M %p", "%H:%M", "%I:%M%p"]
            start_dt = None
            
            for fmt in formats:
                try:
                    t = datetime.strptime(start_time_str, fmt).time()
                    start_dt = datetime.combine(datetime.today(), t)
                    break
                except ValueError:
                    continue
            
            if start_dt:
                finish_dt = start_dt + timedelta(seconds=total_seconds)
                
                day_label = ""
                if finish_dt.date() > start_dt.date():
                    days_diff = (finish_dt.date() - start_dt.date()).days
                    day_label = f" (+{days_diff} day)"

                print(f"Start Time:     {start_dt.strftime('%I:%M:%S %p')}")
                print(f"Finish Time:    {finish_dt.strftime('%I:%M:%S %p')}{day_label}")
            else:
                print("Invalid time format provided. Showing duration only.")
        
        print("="*30)

    except ValueError as e:
        print(f"\nError: {e}")
    except Exception as e:
        print(f"\nAn unexpected error occurred: {e}")

if __name__ == "__main__":
    main()