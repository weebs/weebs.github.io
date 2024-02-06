﻿module Fable

open System.Runtime.InteropServices


type Ref<'t>(initValue: 't) =
    let mutable value = initValue
    member this.Value = value
    member this.SetValue _value = value <- _value
module Hash =
    let hashValue (pointer: nativeint) (length: int) : int =
        let pointer: nativeptr<byte> = NativeInterop.NativePtr.ofNativeInt pointer
        let mutable hash = 13
        let M = 7
        for i in 0..length - 1 do
            let value = NativeInterop.NativePtr.get pointer i
            hash <- (M * hash) + int value
        if hash < 0 then -1 * hash else hash
module System =
    type String(chars: nativeptr<char>) =
        let length =
            let mutable count = 0
            while NativeInterop.NativePtr.get chars count <> '\000' do
                count <- count + 1
            count
        let data: nativeptr<char> =
            Marshal.AllocHGlobal (length + 1)
            |> NativeInterop.NativePtr.ofNativeInt
        do
            for i in 0..length - 1 do
                NativeInterop.NativePtr.set data i (NativeInterop.NativePtr.get chars i)
            NativeInterop.NativePtr.set data length '\000'
        member this.Substring (startIndex: int) =
            String(NativeInterop.NativePtr.add data startIndex)
        override this.Finalize() =
            Marshal.FreeHGlobal (NativeInterop.NativePtr.toNativeInt data)
    // module Runtime =
    //     module InteropServices =
    //         module Marshal =
    //             let FreeHGlobal (ptr: nativeint) =
    //                 ()
    module Collections =
        module Generic =
            type List<'t>() =
                let mutable items: 't[] = Array.zeroCreate 0
                let mutable n = 0
                member this.Add(item: 't) =
                    if n >= items.Length then
                        let copy = items
                        items <- Array.zeroCreate ((items.Length + 1) * 2)
                        for i in 0..n - 1 do
                            items[i] <- copy[i]
                    items[n] <- item
                    n <- n + 1
                    // printfn $"[{n}] ==> {item}"
                member this.Item
                    with get index = items[index]
                member this.Count = n
            type Dictionary<'key, 'value when 'key: equality>() =
                // let items: 'value[] = Array.zeroCreate 0
                let buckets: ('key * 'value)[][] = Array.zeroCreate 8
                let counts: int[] = Array.zeroCreate 8
                do
                    for i in 0..7 do
                        buckets[i] <- Array.zeroCreate 1
                        counts[i] <- 0
                // let keys: 'key[][] = Array.zeroCreate 0
                // let buckets: 'value[][] = Array.zeroCreate 0
                member this.Item
                    with get (key: 'key) : 'value =
                        let hash = key.GetHashCode()
                        let bucketIndex = hash % buckets.Length
                        let mutable found = false
                        let mutable index = 0
                        while not found && index < counts[bucketIndex] do
                            let value: 'key = fst (buckets[bucketIndex][index])
                            // let value: 'key = keys[bucketIndex][index]
                            if value = key then
                                found <- true
                            index <- index + 1
                        if not found then
                            exit 1337
                            // todo: exit 1337
                        snd (buckets[bucketIndex][index - 1])
                member this.Add (key: 'key, value: 'value) =
                    // let mutable k = key
                    // let hash = Hash.hashValue (NativeInterop.NativePtr.toNativeInt &&k) sizeof<'key>
                    let hash = key.GetHashCode()
                    printfn $"hash = {hash}"
                    printfn $"hash index = {hash % buckets.Length}"
                    let bucketIndex = hash % buckets.Length
                    if counts[bucketIndex] >= buckets[bucketIndex].Length then
                        let bucket = buckets[bucketIndex]
                        let newBucket = Array.zeroCreate (bucket.Length * 2)
                        for i in 0..(counts[bucketIndex]) - 1 do
                            newBucket[i] <- bucket[i]
                        buckets[bucketIndex] <- newBucket
                    buckets.[bucketIndex].[counts[bucketIndex]] <- (key, value)
                    counts[bucketIndex] <- counts[bucketIndex] + 1
